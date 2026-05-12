using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;
using DigitalLoanSystem.Domain.Factories;
using DigitalLoanSystem.Domain.Strategies;
using FluentAssertions;
using Moq;
using Xunit;

namespace DigitalLoanSystem.Tests.ApplicationTests
{
    public class PartialEarlyRepaymentServiceTests
    {
        [Fact]
        public async Task ProcessPartialRepaymentAsync_Should_Close_Loan_When_Payment_Equals_RemainingPrincipal()
        {
            // Arrange
            var loanRepository = new Mock<ILoanRepository>();
            var installmentRepository = new Mock<IInstallmentRepository>();
            var principalCalculationService = new Mock<IPrincipalCalculationService>();
            var paymentGateway = new Mock<IPaymentGateway>();
            var strategyFactory = new Mock<ILoanStrategyFactory>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                LoanType = LoanType.Personal,
                InterestRate = 2.0m,
                TermInMonths = 12,
                Status = LoanStatus.Active
            };

            loan.Installments.Add(new Installment
            {
                LoanId = loan.Id,
                InstallmentNumber = 1,
                Amount = 1000m,
                DueDate = DateTime.Now.AddMonths(1),
                Status = InstallmentStatus.Unpaid
            });

            loanRepository.Setup(r => r.GetByIdWithAllInstallmentsAsync(loan.Id)).ReturnsAsync(loan);
            principalCalculationService.Setup(s => s.CalculateRemainingPrincipal(loan)).Returns(1000m);
            paymentGateway.Setup(g => g.ProcessPaymentAsync(1000m, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            unitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var service = new PartialEarlyRepaymentService(
                loanRepository.Object,
                installmentRepository.Object,
                principalCalculationService.Object,
                paymentGateway.Object,
                strategyFactory.Object,
                unitOfWork.Object);

            var request = new PartialEarlyRepaymentRequestDto(
                loan.Id,
                1000m,
                RestructuringOption.ChangeNewTerm,
                3,
                "123",
                "12/30");

            // Act
            var response = await service.ProcessPartialRepaymentAsync(request);

            // Assert
            response.IsEarlyClosureFlag.Should().BeTrue();
            loan.Status.Should().Be(LoanStatus.Closed);
            installmentRepository.Verify(r => r.CancelUnpaidInstallmentsByLoanIdAsync(loan.Id), Times.Once);
            installmentRepository.Verify(r => r.AddInstallmentsAsync(It.IsAny<IEnumerable<Installment>>()), Times.Never);
            installmentRepository.Verify(r => r.AddPaymentAsync(It.IsAny<Payment>()), Times.Once);
            unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ProcessPartialRepaymentAsync_Should_Create_New_Installments_When_Restructuring()
        {
            // Arrange
            var loanRepository = new Mock<ILoanRepository>();
            var installmentRepository = new Mock<IInstallmentRepository>();
            var principalCalculationService = new Mock<IPrincipalCalculationService>();
            var paymentGateway = new Mock<IPaymentGateway>();
            var strategyFactory = new Mock<ILoanStrategyFactory>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                LoanType = LoanType.Personal,
                InterestRate = 2.0m,
                TermInMonths = 12,
                Status = LoanStatus.Active
            };

            loanRepository.Setup(r => r.GetByIdWithAllInstallmentsAsync(loan.Id)).ReturnsAsync(loan);
            principalCalculationService.Setup(s => s.CalculateRemainingPrincipal(loan)).Returns(9000m);
            paymentGateway.Setup(g => g.ProcessPaymentAsync(1000m, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            strategyFactory.Setup(f => f.CreateStrategy(loan.LoanType)).Returns(new PersonalLoanStrategy());
            unitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var service = new PartialEarlyRepaymentService(
                loanRepository.Object,
                installmentRepository.Object,
                principalCalculationService.Object,
                paymentGateway.Object,
                strategyFactory.Object,
                unitOfWork.Object);

            var request = new PartialEarlyRepaymentRequestDto(
                loan.Id,
                1000m,
                RestructuringOption.ChangeNewTerm,
                3,
                "123",
                "12/30");

            // Act
            var response = await service.ProcessPartialRepaymentAsync(request);

            // Assert
            response.IsEarlyClosureFlag.Should().BeFalse();
            response.NewInstallmentPlan.Should().NotBeNull();
            response.NewInstallmentPlan!.Should().HaveCount(3);
            loan.PrincipalAmount.Should().Be(8000m);
            loan.TermInMonths.Should().Be(3);
            installmentRepository.Verify(r => r.CancelUnpaidInstallmentsByLoanIdAsync(loan.Id), Times.Once);
            installmentRepository.Verify(r => r.AddInstallmentsAsync(It.Is<IEnumerable<Installment>>(list =>
                list.Count() == 3 && list.All(i => i.LoanId == loan.Id))), Times.Once);
            installmentRepository.Verify(r => r.AddPaymentAsync(It.IsAny<Payment>()), Times.Once);
            unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
