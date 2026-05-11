using DigitalLoanSystem.Domain.Enums;
using DigitalLoanSystem.Domain.Strategies;

namespace DigitalLoanSystem.Domain.Factories
{
    public interface ILoanStrategyFactory
    {
        ILoanCalculationStrategy CreateStrategy(LoanType loanType);
    }
}