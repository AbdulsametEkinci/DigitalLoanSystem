namespace DigitalLoanSystem.Domain.Strategies
{
    // Interface
    public interface ILoanCalculationStrategy
    {
        decimal CalculateMonthlyInstallment(decimal principal, decimal interestRate, int termInMonths);
    }
}