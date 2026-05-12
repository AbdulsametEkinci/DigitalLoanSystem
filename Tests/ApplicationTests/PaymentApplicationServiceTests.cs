using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace DigitalLoanSystem.Tests.ApplicationTests
{
    public class PaymentApplicationServiceTests
    {
        [Fact]
        public async Task MakePaymentAsync_Should_Throw_When_Installment_Already_Paid()
        {
            // Arrange
            var installmentRepository = new Mock<IInstallmentRepository>();
            var loanRepository = new Mock<ILoanRepository>();
            var paymentGateway = new Mock<IPaymentGateway>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var loan = new Loan { Status = LoanStatus.Active };
            var installment = new Installment
            {
                Id = Guid.NewGuid(),
                Loan = loan,
                Status = InstallmentStatus.Paid,
                Amount = 1500m,
                DueDate = DateTime.Now.AddDays(-1)
            };

            loan.Installments.Add(installment);
            installmentRepository.Setup(r => r.GetByIdWithLoanAsync(installment.Id)).ReturnsAsync(installment);

            var service = new PaymentApplicationService(
                installmentRepository.Object,
                loanRepository.Object,
                paymentGateway.Object,
                unitOfWork.Object);

            var request = new PaymentRequestDto(installment.Id, "1234567890123456", "12/30");

            // Act
            Func<Task> act = () => service.MakePaymentAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*taksit zaten ödenmiş*");
            paymentGateway.Verify(g => g.ProcessPaymentAsync(It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task MakePaymentAsync_Should_Throw_When_PaymentGateway_Fails()
        {
            // Arrange
            var installmentRepository = new Mock<IInstallmentRepository>();
            var loanRepository = new Mock<ILoanRepository>();
            var paymentGateway = new Mock<IPaymentGateway>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var loan = new Loan { Status = LoanStatus.Active };
            var installment = new Installment
            {
                Id = Guid.NewGuid(),
                Loan = loan,
                Status = InstallmentStatus.Unpaid,
                Amount = 1500m,
                DueDate = DateTime.Now.AddDays(10)
            };

            loan.Installments.Add(installment);
            installmentRepository.Setup(r => r.GetByIdWithLoanAsync(installment.Id)).ReturnsAsync(installment);
            paymentGateway.Setup(g => g.ProcessPaymentAsync(installment.Amount, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var service = new PaymentApplicationService(
                installmentRepository.Object,
                loanRepository.Object,
                paymentGateway.Object,
                unitOfWork.Object);

            var request = new PaymentRequestDto(installment.Id, "1234567890123456", "12/30");

            // Act
            Func<Task> act = () => service.MakePaymentAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Ödeme alınamadı*");
            installment.Status.Should().Be(InstallmentStatus.Unpaid);
            installmentRepository.Verify(r => r.AddPaymentAsync(It.IsAny<Payment>()), Times.Never);
            unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task MakePaymentAsync_Should_Mark_Installment_Paid_When_Payment_Succeeds()
        {
            // Arrange
            var installmentRepository = new Mock<IInstallmentRepository>();
            var loanRepository = new Mock<ILoanRepository>();
            var paymentGateway = new Mock<IPaymentGateway>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var loan = new Loan { Status = LoanStatus.Active };
            var installment = new Installment
            {
                Id = Guid.NewGuid(),
                Loan = loan,
                Status = InstallmentStatus.Unpaid,
                Amount = 1500m,
                DueDate = DateTime.Now.AddDays(10)
            };

            loan.Installments.Add(installment);
            installmentRepository.Setup(r => r.GetByIdWithLoanAsync(installment.Id)).ReturnsAsync(installment);
            paymentGateway.Setup(g => g.ProcessPaymentAsync(installment.Amount, It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);
            unitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            var service = new PaymentApplicationService(
                installmentRepository.Object,
                loanRepository.Object,
                paymentGateway.Object,
                unitOfWork.Object);

            var request = new PaymentRequestDto(installment.Id, "1234567890123456", "12/30");

            // Act
            var response = await service.MakePaymentAsync(request);

            // Assert
            installment.IsPaid.Should().BeTrue();
            installment.Payment.Should().NotBeNull();
            response.PaidAmount.Should().Be(installment.Amount);
            installmentRepository.Verify(r => r.AddPaymentAsync(It.Is<Payment>(p =>
                p.InstallmentId == installment.Id && p.AmountPaid == installment.Amount)), Times.Once);
            unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
