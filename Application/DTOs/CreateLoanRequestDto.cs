using System;
using DigitalLoanSystem.Domain.Enums;

namespace DigitalLoanSystem.Application.DTOs
{
    // Arayüzden (React) gelecek olan basit veri taşıyıcısı
    public record CreateLoanRequestDto(
        Guid CustomerId,
        LoanType LoanType,
        decimal PrincipalAmount,
        int TermInMonths
    );

    // Kredi onaylandıktan sonra React'e döneceğimiz güvenli veri (DTO)
    public record LoanResponseDto(
        Guid LoanId,
        decimal PrincipalAmount,
        decimal InterestRate,
        int TermInMonths,
        decimal TotalAmountToPay,
        string Status,
        string Message
    );
}