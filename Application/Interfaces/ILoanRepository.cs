using System.Threading.Tasks;
using DigitalLoanSystem.Domain.Entities;
using System.Collections.Generic;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ILoanRepository
    {
        Task AddAsync(Loan loan);
        Task<IEnumerable<Loan>> GetActiveLoansWithInstallmentsAsync(Guid customerId);
        
        /// <summary>
        /// Tüm taksitleri (ödenmemiş, ödenen, iptal edilen) ile krediyi yükler.
        /// Yeniden yapılandırma ve audit işlemleri için gereklidir.
        /// </summary>
        Task<Loan?> GetByIdWithAllInstallmentsAsync(Guid loanId);
    }
}