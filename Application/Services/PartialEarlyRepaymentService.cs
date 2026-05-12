using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;
using DigitalLoanSystem.Domain.Factories;

namespace DigitalLoanSystem.Application.Services
{
    public class PartialEarlyRepaymentService : IPartialEarlyRepaymentService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IInstallmentRepository _installmentRepository;
        private readonly IPrincipalCalculationService _principalCalculationService;
        private readonly IPaymentGateway _paymentGateway;
        private readonly ILoanStrategyFactory _strategyFactory;
        private readonly IUnitOfWork _unitOfWork;

        public PartialEarlyRepaymentService(
            ILoanRepository loanRepository,
            IInstallmentRepository installmentRepository,
            IPrincipalCalculationService principalCalculationService,
            IPaymentGateway paymentGateway,
            ILoanStrategyFactory strategyFactory,
            IUnitOfWork unitOfWork)
        {
            _loanRepository = loanRepository;
            _installmentRepository = installmentRepository;
            _principalCalculationService = principalCalculationService;
            _paymentGateway = paymentGateway;
            _strategyFactory = strategyFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task<RestructuringPreviewDto> GetRestructuringOptionsAsync(Guid loanId)
        {
            var loan = await _loanRepository.GetByIdWithAllInstallmentsAsync(loanId);
            if (loan == null)
                throw new Exception("Kredi bulunamadı.");

            if (loan.Status != LoanStatus.Active)
                throw new Exception("Bu kredi aktif değildir. Yeniden yapılandırma yapamazsınız.");

            decimal remainingPrincipal = _principalCalculationService.CalculateRemainingPrincipal(loan);
            var strategy = _strategyFactory.CreateStrategy(loan.LoanType);

            // Mevcut vadeyi tut, taksit tutarını düşür
            var paidCount = loan.Installments.Count(i => i.IsPaid);
            int remainingMonths = loan.TermInMonths - paidCount;
            decimal monthlyAmountOptionA = strategy.CalculateMonthlyInstallment(
                remainingPrincipal, 
                loan.InterestRate, 
                remainingMonths);
            decimal totalPayableOptionA = remainingPrincipal * (1 + (loan.InterestRate / 100) * remainingMonths);
            decimal interestOptionA = totalPayableOptionA - remainingPrincipal;

            var options = new List<RestructuringOptionDto>
            {
                new RestructuringOptionDto(
                    RestructuringOption.KeepCurrentTerm,
                    remainingMonths,
                    monthlyAmountOptionA,
                    totalPayableOptionA,
                    interestOptionA
                )
            };

            return new RestructuringPreviewDto(
                remainingPrincipal,
                0m, // PaymentAmount akan istemci tarafından belirtilecek
                remainingPrincipal, // NewPrincipal örnek olarak
                options
            );
        }

        public async Task<PartialRepaymentResponseDto> ProcessPartialRepaymentAsync(PartialEarlyRepaymentRequestDto requestDto)
        {
            // Validasyon
            if (requestDto.PaymentAmount <= 0)
                throw new Exception("Ödeme tutarı sıfırdan büyük olmalıdır.");

            // Krediyi yükle
            var loan = await _loanRepository.GetByIdWithAllInstallmentsAsync(requestDto.LoanId);
            if (loan == null)
                throw new Exception("Kredi bulunamadı.");

            if (loan.Status != LoanStatus.Active)
                throw new Exception("Bu kredi aktif değildir.");

            // Kalan anapara hesapla
            decimal remainingPrincipal = _principalCalculationService.CalculateRemainingPrincipal(loan);

            // Ödeme tutarı kontrolü
            if (requestDto.PaymentAmount > remainingPrincipal)
                throw new Exception($"Ara ödeme tutarı kalan anaparadan ({remainingPrincipal} TL) büyük olamaz.");

            // Ödeme altyapısına istek at
            bool isPaymentSuccess = await _paymentGateway.ProcessPaymentAsync(
                requestDto.PaymentAmount, 
                requestDto.CardNumber, 
                requestDto.ExpiryDate);

            if (!isPaymentSuccess)
                throw new Exception("Ödeme alınamadı. Bakiye yetersiz veya kart geçersiz.");

            // Payment kaydı oluştur (InstallmentId: null çünkü ara ödeme herhangi bir taksite bağlı değil)
            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                InstallmentId = null, // Erken ödeme için nullable
                PaymentDate = DateTime.Now,
                AmountPaid = requestDto.PaymentAmount
            };

            await _installmentRepository.AddPaymentAsync(payment);

            // 7. Yeni anapara hesapla
            decimal newPrincipal = remainingPrincipal - requestDto.PaymentAmount;

            // 8. Erken Kapatma Kontrolü
            bool isEarlyClosureFlag = Math.Abs(newPrincipal) < 0.01m;

            if (isEarlyClosureFlag)
            {
                // Tamamen kapatma senaryosu: eski taksitleri doğrudan DB'de cancel et
                await _installmentRepository.CancelUnpaidInstallmentsByLoanIdAsync(requestDto.LoanId);
                loan.Status = LoanStatus.Closed;
                
                await _unitOfWork.CommitAsync();

                return new PartialRepaymentResponseDto(
                    true,
                    "Krediniz başarıyla kapatılmıştır.",
                    true,
                    null,
                    payment.Id
                );
            }

            // Yeniden yapılandırma
            if (requestDto.RestructuringOption == RestructuringOption.ChangeNewTerm && requestDto.NewTermInMonths.HasValue && requestDto.NewTermInMonths <= 0)
                throw new Exception("Yeni vade sıfırdan büyük olmalıdır.");

            int newTermInMonths = requestDto.RestructuringOption == RestructuringOption.KeepCurrentTerm
                ? loan.TermInMonths - loan.Installments.Count(i => i.IsPaid)
                : requestDto.NewTermInMonths ?? throw new Exception("Yeni vade belirtilmelidir.");

            // Eski taksitleri DB'de doğrudan softdelete 
            await _installmentRepository.CancelUnpaidInstallmentsByLoanIdAsync(requestDto.LoanId);
            
            // Loan parametrelerini güncelle
            loan.PrincipalAmount = newPrincipal;
            loan.TermInMonths = newTermInMonths;
            
            // Yeni taksitleri oluştur (orijinal faiz oranıyla)
            var strategy = _strategyFactory.CreateStrategy(loan.LoanType);
            decimal monthlyAmount = strategy.CalculateMonthlyInstallment(newPrincipal, loan.InterestRate, newTermInMonths);
            decimal totalInterest = newPrincipal * (loan.InterestRate / 100) * newTermInMonths;
            decimal totalAmountToBePaid = newPrincipal + totalInterest;
            decimal accumulatedAmount = 0;
            var newInstallments = new List<Installment>();

            for (int i = 1; i <= newTermInMonths; i++)
            {
                decimal currentInstallmentAmount = monthlyAmount;

                if (i == newTermInMonths)
                {
                    currentInstallmentAmount = totalAmountToBePaid - accumulatedAmount;
                }

                newInstallments.Add(new Installment
                {
                    Id = Guid.NewGuid(),
                    LoanId = loan.Id,
                    InstallmentNumber = i,
                    Amount = currentInstallmentAmount,
                    DueDate = DateTime.Now.AddMonths(i),
                    Status = InstallmentStatus.Unpaid
                });

                accumulatedAmount += currentInstallmentAmount;
            }

            await _installmentRepository.AddInstallmentsAsync(newInstallments);

            
            await _unitOfWork.CommitAsync();

            // Yeni taksit planını DTO'ya çevir (sadece Status != Canceled olanları göster)
            var newInstallmentPlan = newInstallments
                .OrderBy(i => i.DueDate)
                .Select(i => new InstallmentSummaryDto(
                    i.Id,
                    i.LoanId,
                    i.InstallmentNumber,
                    i.Amount,
                    i.DueDate,
                    i.IsDelayed,
                    i.IsPaid,
                    i.IsPaid ? "Ödendi" : (i.IsDelayed ? "Gecikmiş" : "Ödenmedi")
                ))
                .ToList();

            return new PartialRepaymentResponseDto(
                true,
                $"Krediniz başarıyla yeniden yapılandırıldı. Yeni vade: {newTermInMonths} ay",
                false,
                newInstallmentPlan,
                payment.Id
            );
        }
    }
}
