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
            // AsNoTracking: Sadece okuma yapacağımız için EF Core nesneleri izlemesin (Performans)
            return await _context.Installments
                .AsNoTracking()
                .Where(i => i.LoanId == loanId)
                .OrderBy(i => i.InstallmentNumber)
                .ToListAsync();
        }
    }
}