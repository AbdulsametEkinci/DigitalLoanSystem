using System.Threading.Tasks;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface ILoanRepository
    {
        // Oluşturulan Kredi (ve içindeki Taksitleri) veritabanına eklemeye yarar
        Task AddAsync(Loan loan);
    }
}