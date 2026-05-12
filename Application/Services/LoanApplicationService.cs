using System;
using System.Linq;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;
using DigitalLoanSystem.Domain.Enums;
using DigitalLoanSystem.Domain.Factories;

namespace DigitalLoanSystem.Application.Services
{
    public class LoanApplicationService : ILoanApplicationService
    {
        // DI
        private readonly ILoanRepository _loanRepository;
        private readonly ICustomerRepository _customerRepository; // Müşterinin TCKN
        private readonly ICreditScoreService _creditScoreService;
        private readonly IPricingEngineService _pricingEngineService;
        private readonly ILoanStrategyFactory _strategyFactory;
        private readonly IUnitOfWork _unitOfWork;

        public LoanApplicationService(
            ILoanRepository loanRepository,
            ICustomerRepository customerRepository,
            ICreditScoreService creditScoreService,
            IPricingEngineService pricingEngineService,
            ILoanStrategyFactory strategyFactory,
            IUnitOfWork unitOfWork)
        {
            _loanRepository = loanRepository;
            _customerRepository = customerRepository;
            _creditScoreService = creditScoreService;
            _pricingEngineService = pricingEngineService;
            _strategyFactory = strategyFactory;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoanResponseDto> ApplyForLoanAsync(CreateLoanRequestDto requestDto)
        {
            // Girdi Validasyonu
            if (requestDto.PrincipalAmount <= 0)
                throw new Exception("Kredi tutarı sıfırdan büyük olmalıdır.");
            
            if (requestDto.TermInMonths <= 0)
                throw new Exception("Kredi vadesi sıfırdan büyük olmalıdır.");

            // Müşteriyi veritabanından bul
            var customer = await _customerRepository.GetByIdAsync(requestDto.CustomerId);
            if (customer == null)
            {
                throw new Exception("Müşteri bulunamadı.");
            }

            // Kredi Skoru Kontrolü
            bool isEligible = _creditScoreService.IsEligibleForLoan(customer.IdentityNumber);
            if (!isEligible)
            {
                throw new Exception("Kredi skorunuz yetersiz olduğu için başvurunuz reddedildi.");
            }

            // Güncel faiz oranı
            decimal currentInterestRate = _pricingEngineService.GetCurrentInterestRate(requestDto.LoanType, customer.IdentityNumber);

            // Strategy seçimi
            var calculationStrategy = _strategyFactory.CreateStrategy(requestDto.LoanType);

            // Loan entity oluştur
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                CustomerId = customer.Id,
                LoanType = requestDto.LoanType,
                PrincipalAmount = requestDto.PrincipalAmount,
                InterestRate = currentInterestRate,
                TermInMonths = requestDto.TermInMonths,
                StartDate = DateTime.Now,
                Status = LoanStatus.Active
            };

            // Taksitleri üret
            loan.GenerateInstallments(calculationStrategy);

            // Kaydet
            await _loanRepository.AddAsync(loan);
            await _unitOfWork.CommitAsync();

            // Yanıt DTO
            decimal totalPayable = loan.Installments.Sum(i => i.Amount);

            return new LoanResponseDto(
                loan.Id,
                loan.PrincipalAmount,
                loan.InterestRate,
                loan.TermInMonths,
                totalPayable,
                "Approved",
                "Krediniz onaylandı ve taksit planınız başarıyla oluşturuldu."
            );
        }
    }
}