using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;

namespace DigitalLoanSystem.Application.Services
{
    public class InstallmentApplicationService : IInstallmentApplicationService
    {
        private readonly IInstallmentRepository _installmentRepository;

        public InstallmentApplicationService(IInstallmentRepository installmentRepository)
        {
            _installmentRepository = installmentRepository;
        }

        public async Task<IEnumerable<InstallmentDto>> GetInstallmentsByLoanIdAsync(Guid loanId)
        {
            var installments = await _installmentRepository.GetByLoanIdAsync(loanId);

            // Taksitlerin "Gecikmiş" olup olmadığını Entity kendisi biliyor.
            // sadece bunu DTO'ya mapliyor.
            var dtoList = installments.Select(i => new InstallmentDto
            (   
                i.Id,
                i.InstallmentNumber,
                i.Amount,
                i.DueDate,
                i.Status == Domain.Enums.InstallmentStatus.Paid 
                    ? "Ödendi" 
                    : (i.IsDelayed ? "Gecikmiş" : "Ödenmedi")
            ));

            return dtoList;
        }
    }
}