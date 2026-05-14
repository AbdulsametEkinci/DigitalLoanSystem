using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ICustomerApplicationService
    {
        Task<List<GetCustomerDto>> GetAllCustomersAsync();
        Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
        Task<GetCustomerDto> GetCustomerAsync(Guid customerId);
        Task<GetCustomerDto> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto dto);
        Task<CustomerSummaryDto> GetCustomerSummaryAsync(Guid customerId);
        Task<bool> DeleteCustomerAsync(Guid customerId);
    }
}