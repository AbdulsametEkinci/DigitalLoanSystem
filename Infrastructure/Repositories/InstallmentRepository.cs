using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Infrastructure.Data;

namespace DigitalLoanSystem.Infrastructure.Repositories
{
    public class InstallmentRepository : IInstallmentRepository
    {
        private readonly AppDbContext _context;

        public InstallmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Installment>> GetByLoanIdAsync(Guid loanId)
        {
            return await _context.Installments
                .AsNoTracking()
                .Include(i => i.Payment)
                .Where(i => i.LoanId == loanId)
                .OrderBy(i => i.InstallmentNumber)
                .ToListAsync();
        }

        public async Task<Installment?> GetByIdWithLoanAsync(Guid id)
        {
            return await _context.Installments
                .Include(i => i.Payment)
                .Include(i => i.Loan)
                .ThenInclude(l => l.Installments)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddPaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
        }

        public async Task AddInstallmentsAsync(IEnumerable<Installment> installments)
        {
            await _context.Installments.AddRangeAsync(installments);
        }

        public async Task CancelUnpaidInstallmentsByLoanIdAsync(Guid loanId)
        {
            var unpaidInstallments = await _context.Installments
                .Where(i => i.LoanId == loanId && i.Status == Domain.Enums.InstallmentStatus.Unpaid)
                .ToListAsync();

            foreach (var installment in unpaidInstallments)
            {
                installment.MarkAsCanceled();
            }
        }
    }
}
