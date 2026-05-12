using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Application.Services;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace DigitalLoanSystem.Tests.ApplicationTests
{
    public class CustomerApplicationServiceTests
    {
        [Fact]
        public async Task GetCustomerSummaryAsync_Should_Return_Correct_RemainingPrincipal_And_DelayedCount()
        {
            // Arrange
            var customerRepository = new Mock<ICustomerRepository>();
            var loanRepository = new Mock<ILoanRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                PrincipalAmount = 12000m,
                TermInMonths = 12,
                Status = LoanStatus.Active
            };

            var installments = new List<Installment>();
            for (int i = 1; i <= 12; i++)
            {
                var status = i <= 3 ? InstallmentStatus.Paid : InstallmentStatus.Unpaid;
                var dueDate = i == 4
                    ? DateTime.Now.AddDays(-2)
                    : (i <= 3 ? DateTime.Now.AddMonths(-i) : DateTime.Now.AddMonths(i));

                installments.Add(new Installment
                {
                    LoanId = loan.Id,
                    InstallmentNumber = i,
                    Amount = 1000m,
                    DueDate = dueDate,
                    Status = status
                });
            }

            loan.Installments = installments;

            loanRepository
                .Setup(r => r.GetActiveLoansWithInstallmentsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<Loan> { loan });

            var service = new CustomerApplicationService(
                customerRepository.Object,
                loanRepository.Object,
                unitOfWork.Object);

            var customerId = Guid.NewGuid();

            // Act
            var summary = await service.GetCustomerSummaryAsync(customerId);

            // Assert
            summary.RemainingPrincipal.Should().Be(9000m);
            summary.DelayedInstallmentsCount.Should().Be(1);
            summary.UnpaidInstallments.Should().HaveCount(9);
        }
    }
}
