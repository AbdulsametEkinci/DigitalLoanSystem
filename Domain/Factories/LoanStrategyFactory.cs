using System;
using DigitalLoanSystem.Domain.Enums;
using DigitalLoanSystem.Domain.Strategies;

namespace DigitalLoanSystem.Domain.Factories
{
    public class LoanStrategyFactory : ILoanStrategyFactory
    {
        public ILoanCalculationStrategy CreateStrategy(LoanType loanType)
        {
            return loanType switch
            {
                LoanType.Personal => new PersonalLoanStrategy(),
                LoanType.Education => new EducationLoanStrategy(),
                LoanType.Vehicle => new VehicleLoanStrategy(),
                _ => throw new ArgumentException("Geçersiz kredi türü.")
            };
        }
    }
}