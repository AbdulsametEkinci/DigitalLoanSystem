namespace DigitalLoanSystem.Domain.Enums
{
    public enum InstallmentStatus
    {
        Unpaid = 1,
        Paid = 2
        // Gecikmiş (Delayed) durumu veritabanında tutulmaz, zamana göre dinamik hesaplanmalı.
    }
}