using System;

namespace DigitalLoanSystem.Domain.Strategies
{
    public class PersonalLoanStrategy : ILoanCalculationStrategy
    {
        public decimal CalculateMonthlyInstallment(decimal principal, decimal interestRate, int term)
        {
            decimal totalInterest = principal * (interestRate / 100) * term;
            return Math.Round((principal + totalInterest) / term, 2);
        }
    }
}