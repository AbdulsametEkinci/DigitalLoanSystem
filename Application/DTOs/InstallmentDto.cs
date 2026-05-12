using System;

namespace DigitalLoanSystem.Application.DTOs
{
    public record InstallmentDto
    (
         Guid Id,
         int InstallmentNumber,
         decimal Amount,
         DateTime DueDate,
        
        // Enum yerine frontend'in kolayca basacağı string bir değer dönüyoruz
         string StatusDisplay 
    );
}