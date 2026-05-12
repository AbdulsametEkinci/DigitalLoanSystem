using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IPaymentApplicationService
    {
        Task<PaymentResponseDto> MakePaymentAsync(PaymentRequestDto requestDto);
    }
}