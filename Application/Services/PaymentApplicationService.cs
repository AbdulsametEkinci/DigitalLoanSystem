using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;

namespace DigitalLoanSystem.Application.Services
{
    public class PaymentApplicationService : IPaymentApplicationService
    {
        private readonly IInstallmentRepository _installmentRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentApplicationService(
            IInstallmentRepository installmentRepository,
            ILoanRepository loanRepository,
            IPaymentGateway paymentGateway,
            IUnitOfWork unitOfWork)
        {
            _installmentRepository = installmentRepository;
            _loanRepository = loanRepository;
            _paymentGateway = paymentGateway;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentResponseDto> MakePaymentAsync(PaymentRequestDto requestDto)
        {
            // Taksidi veritabanından çek
            var installment = await _installmentRepository.GetByIdWithLoanAsync(requestDto.InstallmentId);
            
            if (installment == null)
                throw new Exception("Taksit bulunamadı.");

            if (installment.Loan == null)
                throw new Exception("Kredi bilgisi bulunamadı.");

            if (installment.Loan.Status == LoanStatus.Closed)
                throw new Exception("Bu kredi zaten kapatılmış.");

            // Taksit Zaten Ödenmiş Kontrolü
            if (installment.IsPaid)
                throw new Exception("Bu taksit zaten ödenmiş.");

            // UC-4 CONCURRENCY (Eşzamanlılık) KONTROLÜ MANTIĞI:
            // IsPaid kontrolü + Payments(InstallmentId) unique index sayesinde çift ödeme engellenir.

            // Ödeme Altyapısına İstek At (ADAPTER)
            bool isPaymentSuccess = await _paymentGateway.ProcessPaymentAsync(installment.Amount, requestDto.CardNumber, requestDto.ExpiryDate);
            
            if (!isPaymentSuccess)
                throw new Exception("Ödeme alınamadı. Bakiye yetersiz veya kart geçersiz.");

            // Payment Entity
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                InstallmentId = installment.Id,
                PaymentDate = DateTime.Now,
                AmountPaid = installment.Amount
            };

            // Taksidin durumunu güncelle
            installment.MarkAsPaid(payment);

            // Installment zaten EF Tracker'da olduğu için otomatik güncellenecek.
            await _installmentRepository.AddPaymentAsync(payment);

            // Kredinin tüm taksitleri ödendi mi
            // Loan nesnesine "Tüm taksitlerin bittiyse kendini kapat".
            installment.Loan.CheckAndCloseLoanIfFullyPaid();

            // Transaction 
            await _unitOfWork.CommitAsync();

            return new PaymentResponseDto(
                payment.Id,
                payment.AmountPaid,
                "Ödeme başarıyla alındı ve borcunuz düşüldü."
            );
        }
    }
}
