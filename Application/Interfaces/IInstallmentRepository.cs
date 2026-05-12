using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IInstallmentRepository
    {
        Task<IEnumerable<Installment>> GetByLoanIdAsync(Guid loanId);

        Task<Installment?> GetByIdWithLoanAsync(Guid id);
        Task AddPaymentAsync(Payment payment);
        Task AddInstallmentsAsync(IEnumerable<Installment> installments);

        /// <summary>
        /// Belirtilen kredi için ödenmemiş taksitlerin durumunu "Canceled" olarak günceller.
        /// Softdelete mantığı ile veritabanından silmez, sadece Status'u değiştirir.
        /// </summary>
        Task CancelUnpaidInstallmentsByLoanIdAsync(Guid loanId);
    }
}
