namespace DigitalLoanSystem.Domain.Enums
{
    public enum InstallmentStatus
    {
        Unpaid = 1,
        Paid = 2,
        Canceled = 3
        // Gecikmiş (Delayed) durumu veritabanında tutulmaz, zamana göre dinamik hesaplanmalı.
    }
}