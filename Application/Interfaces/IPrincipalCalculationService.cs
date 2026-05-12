using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IPrincipalCalculationService
    {
        /// <summary>
        /// Kredi için o anki Kalan Anapara (Remaining Principal) değerini hesaplar.
        /// Formula: RemainingPrincipal = PrincipalAmount × (TermInMonths - PaidInstallmentsCount) / TermInMonths
        /// </summary>
        decimal CalculateRemainingPrincipal(Loan loan);
    }
}
