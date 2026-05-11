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

        public Loan Loan { get; set; }
        public Payment Payment { get; set; }

        // Info Expert
        public bool IsDelayed => Status == InstallmentStatus.Unpaid && DateTime.Now > DueDate;

        public void MarkAsPaid()
        {
            Status = InstallmentStatus.Paid;
        }
    }
}