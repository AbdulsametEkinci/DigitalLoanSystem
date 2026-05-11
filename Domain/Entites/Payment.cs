using System;

namespace DigitalLoanSystem.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid InstallmentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal AmountPaid { get; set; }

        public Installment Installment { get; set; }
    }
}