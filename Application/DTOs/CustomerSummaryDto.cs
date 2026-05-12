using System;
using System.Collections.Generic;

namespace DigitalLoanSystem.Application.DTOs
{
    public record CustomerSummaryDto(
        decimal TotalRemainingDebt,
        decimal RemainingPrincipal,
        int DelayedInstallmentsCount,
        List<InstallmentSummaryDto> Installments,
        List<InstallmentSummaryDto> UnpaidInstallments
    );

    public record InstallmentSummaryDto(
        Guid LoanId,
        int InstallmentNumber,
        decimal Amount,
        DateTime DueDate,
        bool IsDelayed,
        bool IsPaid,
        string StatusDisplay
    );
}
