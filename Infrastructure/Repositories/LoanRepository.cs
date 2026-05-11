using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
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
    }
}