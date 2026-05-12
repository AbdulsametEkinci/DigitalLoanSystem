using System;
using DigitalLoanSystem.Domain.Enums;

namespace DigitalLoanSystem.Domain.Entities
{
    public class Installment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LoanId { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public InstallmentStatus Status { get; set; }
        public DateTime? CanceledDate { get; set; }

        public Loan Loan { get; set; } = null!;
        public Payment? Payment { get; set; }

        // Info Expert
        public bool IsPaid => Status == InstallmentStatus.Paid || Payment != null;
        public bool IsDelayed => !IsPaid && DateTime.Now > DueDate;

        public void MarkAsPaid(Payment payment)
        {
            Status = InstallmentStatus.Paid;
            Payment = payment;
        }

        public void MarkAsCanceled()
        {
            Status = InstallmentStatus.Canceled;
            CanceledDate = DateTime.Now;
        }
    }
}
