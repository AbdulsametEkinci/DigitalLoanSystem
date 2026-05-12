using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;

namespace DigitalLoanSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentApplicationService _paymentService;

        public PaymentsController(IPaymentApplicationService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// UC-4: Bir taksit için ödeme alır.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> PayInstallment([FromBody] PaymentRequestDto requestDto)
        {
            try
            {
                var response = await _paymentService.MakePaymentAsync(requestDto);
                return Ok(response); // Ödeme başarılı (200 OK)
            }
            catch (Exception ex)
            {
                // Ödeme reddedilirse veya taksit zaten ödenmişse (400 Bad Request)
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}