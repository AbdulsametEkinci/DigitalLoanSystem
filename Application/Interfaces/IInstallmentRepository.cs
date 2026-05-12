using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalLoanSystem.Domain.Entities;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IInstallmentRepository
    {
        Task<IEnumerable<Installment>> GetByLoanIdAsync(Guid loanId);
    }
}