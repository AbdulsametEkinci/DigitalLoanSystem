using System;

namespace DigitalLoanSystem.Application.DTOs
{
    public record CreateCustomerDto
    (
         string IdentityNumber,
         string FullName,
         string Email,
         string PhoneNumber
    );

    public record UpdateCustomerDto
    (
         string FullName,
         string Email,
         string PhoneNumber
    );

    public record GetCustomerDto
    (
         Guid Id,
         string IdentityNumber,
         string FullName,
         string Email,
         string PhoneNumber
    );

    public record CustomerResponseDto
    (
         Guid Id,
         string FullName,
         string Message
    );
}