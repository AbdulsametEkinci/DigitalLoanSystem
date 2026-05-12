using System;
using System.Collections.Generic;
using DigitalLoanSystem.Domain.Enums;

namespace DigitalLoanSystem.Application.DTOs
{
    public enum RestructuringOption
    {
        KeepCurrentTerm = 1,  // Vade aynı kalır, taksit tutarı düşer
        ChangeNewTerm = 2     // Yeni vade belirle, faiz yükü azalır
    }

    public record PartialEarlyRepaymentRequestDto(
        Guid LoanId,
        decimal PaymentAmount,
        RestructuringOption RestructuringOption,
        int? NewTermInMonths,  // Nullable, sadece ChangeNewTerm için gerekli
        string CardNumber,
        string ExpiryDate
    );

    public record RestructuringOptionDto(
        RestructuringOption Option,
        int TermInMonths,
        decimal MonthlyInstallmentAmount,
        decimal TotalAmountToPay,
        decimal TotalInterestToPay
    );

    public record RestructuringPreviewDto(
        decimal RemainingPrincipal,
        decimal PaymentAmount,
        decimal NewPrincipal,
        List<RestructuringOptionDto> Options
    );

    public record PartialRepaymentResponseDto(
        bool Success,
        string Message,
        bool IsEarlyClosureFlag,
        List<InstallmentSummaryDto>? NewInstallmentPlan,
        Guid? PaymentId
    );
}
