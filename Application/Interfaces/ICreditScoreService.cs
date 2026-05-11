namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ICreditScoreService
    {
        // Müşterinin TCKN'sini alır, kredi çekmeye uygun mu döner.
        bool IsEligibleForLoan(string identityNumber);
    }
}