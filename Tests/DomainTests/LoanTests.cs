using System;
using System.Linq;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;
using DigitalLoanSystem.Domain.Strategies;
using FluentAssertions;
using Xunit;

namespace DigitalLoanSystem.Tests.DomainTests
{
    public class LoanTests
    {
        [Fact]
        public void GenerateInstallments_Should_Create_Correct_Number_Of_Installments()
        {
            // Arrange
            var loan = new Loan
            {
                PrincipalAmount = 10000m,
                InterestRate = 2.0m,
                TermInMonths = 3,
                StartDate = DateTime.Now
            };
            var strategy = new PersonalLoanStrategy();

            // Act
            loan.GenerateInstallments(strategy);

            // Assert
            loan.Installments.Should().HaveCount(3);
            loan.Installments.Should().OnlyContain(i => i.Status == InstallmentStatus.Unpaid);
            loan.Installments.Sum(i => i.Amount).Should().Be(10600m);
        }

        [Fact]
        public void CheckAndCloseLoan_Should_Set_Status_To_Closed_When_All_Paid()
        {
            // Arrange
            var loan = new Loan { Status = LoanStatus.Active };
            loan.Installments.Add(new Installment { Status = InstallmentStatus.Paid });
            loan.Installments.Add(new Installment { Status = InstallmentStatus.Paid });

            // Act
            loan.CheckAndCloseLoanIfFullyPaid();

            // Assert
            loan.Status.Should().Be(LoanStatus.Closed);
        }

        [Fact]
        public void IsDelayed_Should_Return_True_If_DueDate_Passed_And_Unpaid()
        {
            // Arrange
            var installment = new Installment
            {
                Status = InstallmentStatus.Unpaid,
                DueDate = DateTime.Now.AddDays(-5) // 5 gün geçmiş
            };

            // Act
            bool isDelayed = installment.IsDelayed;

            // Assert
            isDelayed.Should().BeTrue();
        }
    }
}
