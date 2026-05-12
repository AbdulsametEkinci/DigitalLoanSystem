using System;
using System.Linq;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;

namespace DigitalLoanSystem.Application.Services
{
    public class PrincipalCalculationService : IPrincipalCalculationService
    {
        public decimal CalculateRemainingPrincipal(Loan loan)
        {
            if (loan == null)
                throw new ArgumentNullException(nameof(loan));

            var paidInstallmentsCount = loan.Installments
                .Count(i => i.Status == InstallmentStatus.Paid || i.IsPaid);

            int remainingMonths = loan.TermInMonths - paidInstallmentsCount;

            if (remainingMonths <= 0)
                return 0m;

            decimal remainingPrincipal = loan.PrincipalAmount * remainingMonths / loan.TermInMonths;
            return Math.Max(0m, remainingPrincipal);
        }
    }
}
