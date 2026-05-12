using Microsoft.EntityFrameworkCore;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablolar
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Installment> Installments { get; set; }
        public DbSet<Payment> Payments { get; set; }

        // Fluent API ile Entity Ayarları
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // CUSTOMER
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.IdentityNumber).IsRequired().HasMaxLength(11);
                entity.HasIndex(e => e.IdentityNumber).IsUnique(); // TCKN tekil olmalı
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            });

            // LOAN (1 Müşteri -> Çok Kredi)
            modelBuilder.Entity<Loan>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.PrincipalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.InterestRate).HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.Loans)
                      .HasForeignKey(e => e.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict); // kredi varsa müşteri silinemez
            });

            // INSTALLMENT (1 Kredi -> Çok Taksit)
            modelBuilder.Entity<Installment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.CanceledDate).HasColumnType("datetime(6)").IsRequired(false);

                entity.HasOne(e => e.Loan)
                      .WithMany(l => l.Installments)
                      .HasForeignKey(e => e.LoanId)
                      .OnDelete(DeleteBehavior.Cascade); // Kredi silinirse taksitler silinsin

                entity.HasIndex(e => new { e.LoanId, e.Status }); // UC-6 için soft-delete sorgularının performansı
            });

            // PAYMENT (1 Taksit -> En fazla 1 Ödeme)
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.AmountPaid).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Installment)
                      .WithOne(i => i.Payment)
                      .HasForeignKey<Payment>(e => e.InstallmentId)
                      .OnDelete(DeleteBehavior.Restrict)
                      .IsRequired(false); // Nullable FK: erken ödeme için
            });
        }
    }
}