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
        private readonly IUnitOfWork _unitOfWork;

        public CustomerApplicationService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomerResponseDto> CreateCustomerAsync(CreateCustomerDto dto)
        {
            // İleride buraya "TCKN sistemde zaten var mı?" kontrolü eklenebilir.

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
    }
}