using System;
using System.Text.Json;
using System.Threading.Tasks;
using DigitalLoanSystem.Application.Interfaces;

namespace DigitalLoanSystem.Infrastructure.Adapters
{
    public class LegacyApiPaymentAdapter : IPaymentGateway
    {
        private readonly string ApiUrl = "https://legacy.architectpos-system.com/api/do_trx";

        public async Task<bool> ProcessPaymentAsync(decimal amount, string cardNumber, string expiryDate)
        {
            Console.WriteLine($"[ADAPTER] Temiz veriler alındı: Tutar={amount}, Kart={cardNumber}");

            // Decimal tutarı, karşı API'nin beklediği "String Kuruş" formatına çevir
            string amountInCents = (amount * 100).ToString("0");

            // API'nin beklediği o garip JSON formatı (DTO Mapping)
            var weirdApiPayload = new
            {
                amt_in_cents = amountInCents, // isimlendirme
                pan_num = cardNumber,
                exp_m_y = expiryDate,
                trx_type = 1                  // 1: Satış
            };

            string jsonContent = JsonSerializer.Serialize(weirdApiPayload);
            Console.WriteLine($"[HTTP POST] {ApiUrl} adresine istek atılıyor...");
            Console.WriteLine($"[PAYLOAD] {jsonContent}");

            // Ağ Gecikmesi (HTTP Request atılıyormuş gibi)
            await Task.Delay(1500);

            // API'den dönen garip JSON cevabını simüle et
            // Kural: Kart "4000" ile başlıyorsa "88 (Red)", aksi halde "99 (Başarılı)" dönsün.
            string mockApiResponseJson = cardNumber.StartsWith("4000")
                ? "{\"status_code\": 88, \"msg\": \"DECLINED_BY_BANK\"}"
                : "{\"status_code\": 99, \"msg\": \"SUCCESS_TRX\"}";

            Console.WriteLine($"[HTTP RESPONSE] {mockApiResponseJson}");

            // cevabı parse et
            // Boolean değere çevir
            using JsonDocument document = JsonDocument.Parse(mockApiResponseJson);
            int statusCode = document.RootElement.GetProperty("status_code").GetInt32();

            if (statusCode == 99)
            {
                return true;
            }
            else if (statusCode == 88)
            {
                return false;
            }
            else
            {
                throw new Exception("Banka API'sinden bilinmeyen bir hata kodu döndü!");
            }
        }
    }
}