using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLoanSystem.Domain.Enums;
using DigitalLoanSystem.Domain.Strategies;

namespace DigitalLoanSystem.Domain.Entities
{
    public class Loan
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CustomerId { get; set; }
        public LoanType LoanType { get; set; }
        public decimal PrincipalAmount { get; set; }

        // Dış servisten alınıp buraya atanacak olan sabit faiz!
        public decimal InterestRate { get; set; }

        public int TermInMonths { get; set; }
        public DateTime StartDate { get; set; }
        public LoanStatus Status { get; set; }

        public Customer? Customer { get; set; }
        public ICollection<Installment> Installments { get; set; } = new List<Installment>();

        // Creator (Taksitleri üretir)
        public void GenerateInstallments(ILoanCalculationStrategy strategy)
        {
            decimal monthlyAmount = strategy.CalculateMonthlyInstallment(PrincipalAmount, InterestRate, TermInMonths);

            decimal totalInterest = PrincipalAmount * (InterestRate / 100) * TermInMonths;
            decimal totalAmountToBePaid = PrincipalAmount + totalInterest;

            decimal accumulatedAmount = 0; // O ana kadar yazılan taksitlerin toplamı

            for (int i = 1; i <= TermInMonths; i++)
            {
                decimal currentInstallmentAmount = monthlyAmount;

                // Eğer son taksitse, toplam tutardan o ana kadar hesaplananları çıkar ve kalanı yaz.
                if (i == TermInMonths)
                {
                    currentInstallmentAmount = totalAmountToBePaid - accumulatedAmount;
                }

                this.Installments.Add(new Installment
                {
                    Id = Guid.NewGuid(),
                    LoanId = this.Id,
                    InstallmentNumber = i,
                    Amount = currentInstallmentAmount,
                    DueDate = this.StartDate.AddMonths(i),
                    Status = InstallmentStatus.Unpaid
                });

                accumulatedAmount += currentInstallmentAmount;
            }
        }

        public void CheckAndCloseLoanIfFullyPaid()
        {
            if (Installments.All(i => i.Status == InstallmentStatus.Paid))
            {
                Status = LoanStatus.Closed;
            }
        }
    }
}
