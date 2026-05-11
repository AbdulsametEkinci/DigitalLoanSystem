using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Infrastructure.Data;

namespace DigitalLoanSystem.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> GetByIdAsync(Guid id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}