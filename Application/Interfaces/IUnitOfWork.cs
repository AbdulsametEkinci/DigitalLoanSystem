using System.Threading.Tasks;

namespace DigitalLoanSystem.Application.Interfaces
{
    // Finansal tutarlılık (Transaction) için kalıp.
    // İşlemlerin tümü başarılı olmadan veritabanına Commit atılmaz.
    public interface IUnitOfWork
    {
        // Kaydetme işlemi başarılıysa etkilenen satır sayısını döner
        Task<int> CommitAsync();
    }
}