using System;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.DTOs;

namespace DigitalLoanSystem.Application.Interfaces
{
    public interface IPartialEarlyRepaymentService
    {
        /// <summary>
        /// Müşterinin yeniden yapılandırma (restructuring) seçeneklerini hesaplar ve döner.
        /// Kalan anaparayı ve iki senaryoyu (vade sabit / vade değişken) sunar.
        /// </summary>
        Task<RestructuringPreviewDto> GetRestructuringOptionsAsync(Guid loanId);

        /// <summary>
        /// Kısmi erken ödeme işlemini gerçekleştirir:
        /// - Ödeme altyapısında tahsil yapar
        /// - Eski taksitleri softdelete yapar (Status = Canceled)
        /// - Müşterinin seçtiği seçeneğe göre yeni taksit planı oluşturur
        /// - Tüm işlemleri tek bir transaction'da veritabanına yazar
        /// </summary>
        Task<PartialRepaymentResponseDto> ProcessPartialRepaymentAsync(PartialEarlyRepaymentRequestDto requestDto);
    }
}
