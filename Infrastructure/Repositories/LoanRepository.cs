using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using DigitalLoanSystem.Infrastructure.Data;

namespace DigitalLoanSystem.Infrastructure.Repositories
{
    public class LoanRepository : ILoanRepository
    {
        private readonly AppDbContext _context;

        public LoanRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Loan loan)
        {
            // Bu işlem sadece bellekte (EF Core Tracker) ekler. 
            // Veritabanına asıl yazma işlemini UnitOfWork yapacak.
            await _context.Loans.AddAsync(loan);
        }

        public async Task<IEnumerable<Loan>> GetActiveLoansWithInstallmentsAsync(Guid customerId)
        {
            // readonly oldugu için AsNoTracking
            return await _context.Loans
                .AsNoTracking()
                .Include(l => l.Installments)
                .ThenInclude(i => i.Payment)
                .Where(l => l.CustomerId == customerId && l.Status == Domain.Enums.LoanStatus.Active)
                .ToListAsync();
        }

        public async Task<Loan?> GetByIdWithAllInstallmentsAsync(Guid loanId)
        {
            // Yeniden yapılandırma için tüm taksitleri (paid, unpaid, canceled) yükle
            // Tracking aktif çünkü update
            return await _context.Loans
                .Include(l => l.Customer)  // FK validation için Customer'ı da yükle
                .Include(l => l.Installments)
                .ThenInclude(i => i.Payment)
                .FirstOrDefaultAsync(l => l.Id == loanId);
        }
    }
}
