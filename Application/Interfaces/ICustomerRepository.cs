using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ICustomerRepository
    {
        // Gelen ID'ye göre veritabanından müşteriyi bulup getirecek
        Task<Customer> GetByIdAsync(Guid id);
    }
}