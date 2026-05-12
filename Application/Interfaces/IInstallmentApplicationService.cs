using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IInstallmentApplicationService
    {
        Task<IEnumerable<InstallmentDto>> GetInstallmentsByLoanIdAsync(Guid loanId);
    }
}