using System.Threading.Tasks;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Infrastructure.Data;

namespace DigitalLoanSystem.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> CommitAsync()
        {
            // Tüm SaveChanges işlemleri buradan tek seferde yönetilir.
            // İşlem sırasında hata çıkarsa veritabanına hiçbir şey yazılmaz (Rollback).
            return await _context.SaveChangesAsync();
        }
    }
}