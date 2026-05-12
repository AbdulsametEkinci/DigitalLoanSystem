using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ICustomerApplicationService
    {
        Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto);
        Task<CustomerSummaryDto> GetCustomerSummaryAsync(Guid customerId);
        Task<bool> DeleteCustomerAsync(Guid customerId);
    }
}