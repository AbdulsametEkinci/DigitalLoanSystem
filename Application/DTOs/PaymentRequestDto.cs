using System;

namespace DigitalLoanSystem.Application.DTOs
{
    public record PaymentRequestDto(
        Guid InstallmentId,
        string CardNumber,
        string ExpiryDate
    );

    public record PaymentResponseDto(
        Guid PaymentId,
        decimal PaidAmount,
        string Message
    );
}
