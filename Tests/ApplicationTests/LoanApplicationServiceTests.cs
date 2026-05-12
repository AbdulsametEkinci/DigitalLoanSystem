using System;
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
    public class LoanApplicationServiceTests
    {
        [Fact]
        public async Task ApplyForLoanAsync_Should_Throw_When_CreditScore_Not_Eligible()
        {
            // Arrange
            var loanRepository = new Mock<ILoanRepository>();
            var customerRepository = new Mock<ICustomerRepository>();
            var creditScoreService = new Mock<ICreditScoreService>();
            var pricingEngineService = new Mock<IPricingEngineService>();
            var strategyFactory = new Mock<ILoanStrategyFactory>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                IdentityNumber = "12345678901",
                FullName = "Test User",
                Email = "test@example.com",
                PhoneNumber = "05555555555"
            };

            customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            creditScoreService.Setup(s => s.IsEligibleForLoan(customer.IdentityNumber)).Returns(false);

            var service = new LoanApplicationService(
                loanRepository.Object,
                customerRepository.Object,
                creditScoreService.Object,
                pricingEngineService.Object,
                strategyFactory.Object,
                unitOfWork.Object);

            var request = new CreateLoanRequestDto(customer.Id, LoanType.Personal, 10000m, 12);

            // Act
            Func<Task> act = () => service.ApplyForLoanAsync(request);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*reddedildi*");
            loanRepository.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Never);
            unitOfWork.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task ApplyForLoanAsync_Should_Set_InterestRate_From_PricingEngine()
        {
            // Arrange
            var loanRepository = new Mock<ILoanRepository>();
            var customerRepository = new Mock<ICustomerRepository>();
            var creditScoreService = new Mock<ICreditScoreService>();
            var pricingEngineService = new Mock<IPricingEngineService>();
            var strategyFactory = new Mock<ILoanStrategyFactory>();
            var unitOfWork = new Mock<IUnitOfWork>();

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                IdentityNumber = "12345678901",
                FullName = "Test User",
                Email = "test@example.com",
                PhoneNumber = "05555555555"
            };

            customerRepository.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);
            creditScoreService.Setup(s => s.IsEligibleForLoan(customer.IdentityNumber)).Returns(true);
            pricingEngineService.Setup(s => s.GetCurrentInterestRate(LoanType.Personal, customer.IdentityNumber)).Returns(1.85m);
            strategyFactory.Setup(f => f.CreateStrategy(LoanType.Personal)).Returns(new PersonalLoanStrategy());
            unitOfWork.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            Loan? capturedLoan = null;
            loanRepository
                .Setup(r => r.AddAsync(It.IsAny<Loan>()))
                .Callback<Loan>(loan => capturedLoan = loan)
                .Returns(Task.CompletedTask);

            var service = new LoanApplicationService(
                loanRepository.Object,
                customerRepository.Object,
                creditScoreService.Object,
                pricingEngineService.Object,
                strategyFactory.Object,
                unitOfWork.Object);

            var request = new CreateLoanRequestDto(customer.Id, LoanType.Personal, 10000m, 12);

            // Act
            var response = await service.ApplyForLoanAsync(request);

            // Assert
            capturedLoan.Should().NotBeNull();
            capturedLoan!.InterestRate.Should().Be(1.85m);
            response.InterestRate.Should().Be(1.85m);
            loanRepository.Verify(r => r.AddAsync(It.IsAny<Loan>()), Times.Once);
            unitOfWork.Verify(u => u.CommitAsync(), Times.Once);
        }
    }
}
