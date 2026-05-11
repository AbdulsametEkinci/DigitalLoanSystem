using DigitalLoanSystem.Domain.Enums;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IPricingEngineService
    {
        // Kredi tipine ve müşteri riskine göre güncel faiz oranını döner.
        decimal GetCurrentInterestRate(LoanType loanType, string identityNumber);
    }
}