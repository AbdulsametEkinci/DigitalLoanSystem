using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ILoanApplicationService
    {
        // Kredi başvurusu yapacak metot
        Task<LoanResponseDto> ApplyForLoanAsync(CreateLoanRequestDto requestDto);
    }
}