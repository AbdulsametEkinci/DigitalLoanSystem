using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Application.Services
{
    // Sadece müşteri işlerinden sorumlu
    public class CustomerApplicationService : ICustomerApplicationService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerApplicationService(ICustomerRepository customerRepository, ILoanRepository loanRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _loanRepository = loanRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto)
        {
            // Duplicate TCKN kontrolü
            var existingCustomer = await _customerRepository.GetByIdentityNumberAsync(dto.IdentityNumber);
            if (existingCustomer != null)
                throw new InvalidOperationException($"TCKN '{dto.IdentityNumber}' zaten sistemde kayıtlı.");

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                IdentityNumber = dto.IdentityNumber,
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            await _customerRepository.AddAsync(customer);
            await _unitOfWork.CommitAsync();

            return new CustomerResponseDto
            (
                customer.Id,
                customer.FullName,
                "Müşteri başarıyla oluşturuldu."
            );
        }

        public async Task<CustomerSummaryDto> GetCustomerSummaryAsync(Guid customerId)
        {
            var totalRemainingDebt = 0m;
            var remainingPrincipal = 0m;
            var delayedInstallmentsCount = 0;
            var installments = new List<InstallmentSummaryDto>();
            var unpaidInstallments = new List<InstallmentSummaryDto>();

            // Müşterinin aktif kredilerini ve taksitlerini çek
            var activeLoans = await _loanRepository.GetActiveLoansWithInstallmentsAsync(customerId);

            // Aktif kredi yoksa, sıfır değerlerle dön.
            if (activeLoans == null || !activeLoans.Any())
            {
                return new CustomerSummaryDto(
                    totalRemainingDebt,
                    remainingPrincipal,
                    delayedInstallmentsCount,
                    installments,
                    unpaidInstallments
                );
            }

            // Finansal Hesaplamalar
            foreach (var loan in activeLoans)
            {
                var orderedInstallments = loan.Installments.OrderBy(i => i.DueDate).ToList();
                var unpaid = orderedInstallments.Where(i => !i.IsPaid).ToList();
                var paidInstallmentsCount = orderedInstallments.Count(i => i.IsPaid);

                // A. Toplam Kredi Borcu (sadece Status 1 olan taksitler)
                totalRemainingDebt += unpaid.Where(i => i.Status == Domain.Enums.InstallmentStatus.Unpaid).Sum(i => i.Amount);

                // B. Kalan Anapara (Ödenen taksit sayısı oranında anapara azalır)
                // Her ödenen taksit, anapranın 1/TermInMonths'unu emekli eder
                int remainingMonths = loan.TermInMonths - paidInstallmentsCount;
                decimal remaining = (remainingMonths > 0) 
                    ? loan.PrincipalAmount * remainingMonths / loan.TermInMonths 
                    : 0m;
                remainingPrincipal += remaining;

                // C. Gecikmiş Taksit Sayısı
                delayedInstallmentsCount += unpaid.Count(i => i.IsDelayed);

                // D. Taksit Listesini DTO'ya dönüştür ve ekle (Ödenen/Ödenmeyen dahil)
                foreach (var installment in orderedInstallments)
                {
                    bool isPaid = installment.IsPaid;
                    bool isDelayed = installment.IsDelayed;
                    bool isCanceled = installment.Status == Domain.Enums.InstallmentStatus.Canceled;
                    string statusDisplay = isPaid ? "Ödendi" : (isDelayed ? "Gecikmiş" : isCanceled ? "İptal Edilmiş" : "Ödenmedi");

                    var dto = new InstallmentSummaryDto(
                        installment.Id,
                        installment.LoanId,
                        installment.InstallmentNumber,
                        installment.Amount,
                        installment.DueDate,
                        isDelayed,
                        isPaid,
                        statusDisplay
                    );

                    installments.Add(dto);

                    if (!isPaid)
                    {
                        unpaidInstallments.Add(dto);
                    }
                }
            }

            // tarihe göre sırla
            installments = installments.OrderBy(i => i.DueDate).ToList();
            unpaidInstallments = unpaidInstallments.OrderBy(i => i.DueDate).ToList();

            // Küsüratları yuvarla
            totalRemainingDebt = Math.Round(totalRemainingDebt, 2);
            remainingPrincipal = Math.Round(remainingPrincipal, 2);

            return new CustomerSummaryDto(
                totalRemainingDebt,
                remainingPrincipal,
                delayedInstallmentsCount,
                installments,
                unpaidInstallments
            );
        }
        public async Task<bool> DeleteCustomerAsync(Guid customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException($"Müşteri ID'si {customerId} bulunamadı.");

            await _customerRepository.DeleteAsync(customer);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public async Task<GetCustomerDto> GetCustomerAsync(Guid customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException($"Müşteri ID'si {customerId} bulunamadı.");

            return new GetCustomerDto(
                customer.Id,
                customer.IdentityNumber,
                customer.FullName,
                customer.Email,
                customer.PhoneNumber
            );
        }

        public async Task<GetCustomerDto> UpdateCustomerAsync(Guid customerId, UpdateCustomerDto dto)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                throw new InvalidOperationException($"Müşteri ID'si {customerId} bulunamadı.");

            customer.FullName = dto.FullName;
            customer.Email = dto.Email;
            customer.PhoneNumber = dto.PhoneNumber;

            await _unitOfWork.CommitAsync();

            return new GetCustomerDto(
                customer.Id,
                customer.IdentityNumber,
                customer.FullName,
                customer.Email,
                customer.PhoneNumber
            );
        }

        public async Task<List<GetCustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(c => new GetCustomerDto(
                c.Id,
                c.IdentityNumber,
                c.FullName,
                c.Email,
                c.PhoneNumber
            )).ToList();
        }

    }
}
