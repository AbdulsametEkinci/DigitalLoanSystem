using DigitalLoanSystem.Application.Interfaces;

namespace DigitalLoanSystem.Infrastructure.Adapters
{
    public class MockCreditScoreAdapter : ICreditScoreService
    {
        public bool IsEligibleForLoan(string identityNumber)
        {
            // MOCK MANTIK: TCKN 11 hanelidir, son hanesi çift olanlar her zaman uygundur gibi basit bir kural.
            if (string.IsNullOrEmpty(identityNumber)) return false;

            int lastDigit = int.Parse(identityNumber.Substring(identityNumber.Length - 1));
            return lastDigit % 2 == 0;
        }
    }
}