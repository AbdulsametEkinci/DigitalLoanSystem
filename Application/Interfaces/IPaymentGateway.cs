using System.Threading.Tasks;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IPaymentGateway
    {
        // Ağ işlemi olduğu için Task ve Async
        Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber, string expiryDate);
    }
}