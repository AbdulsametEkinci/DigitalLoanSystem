using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(Guid id);
        Task<Customer?> GetByIdentityNumberAsync(string identityNumber);
        Task<List<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task DeleteAsync(Customer customer);
    }
}