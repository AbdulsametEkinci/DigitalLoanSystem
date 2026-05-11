using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Enums;

namespace DigitalLoanSystem.Infrastructure.Adapters
{
    public class MockPricingEngineAdapter : IPricingEngineService
    {
        public decimal GetCurrentInterestRate(LoanType loanType, string identityNumber)
        {
            // MOCK MANTIK: Kredi tipine göre statik faiz dönüyoruz. 
            // Gerçekte Merkez Bankası veya Banka Core sisteminden anlık çekilir.
            return loanType switch
            {
                LoanType.Education => 1.25m,  // Eğitim en ucuz
                LoanType.Vehicle => 1.85m,    // Taşıt orta
                LoanType.Personal => 2.45m,   // İhtiyaç en pahalı
                _ => 2.00m
            };
        }
    }
}