using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;

namespace DigitalLoanSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Route: api/loans 
    public class LoansController : ControllerBase
    {
        private readonly ILoanApplicationService _loanService;
        private readonly IPartialEarlyRepaymentService _partialRepaymentService;

        // Controller sadece Service'i bilir, Domain veya DB'yi bilmez.
        public LoansController(
            ILoanApplicationService loanService,
            IPartialEarlyRepaymentService partialRepaymentService)
        {
            _loanService = loanService;
            _partialRepaymentService = partialRepaymentService;
        }

        /// <summary>
        /// UC-2: Yeni kredi başvurusu alır ve taksit planını oluşturur.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ApplyForLoan([FromBody] CreateLoanRequestDto requestDto)
        {
            try
            {
                // İş mantığını çağırıyoruz
                var response = await _loanService.ApplyForLoanAsync(requestDto);

                return Created($"/api/loans/{response.LoanId}", response);
            }
            catch (Exception ex)
            {
                // Kredi skoru yetersizse veya müşteri yoksa 400 Bad Request dönüyoruz.
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// UC-6: Belirtilen kredi için yeniden yapılandırma seçeneklerini getirir.
        /// </summary>
        [HttpGet("{loanId}/restructuring-preview")]
        public async Task<IActionResult> GetRestructuringOptions(Guid loanId)
        {
            if (loanId == Guid.Empty)
                return BadRequest("Geçerli bir Kredi ID'si girmelisiniz.");

            try
            {
                var preview = await _partialRepaymentService.GetRestructuringOptionsAsync(loanId);
                return Ok(preview);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// UC-6: Kısmi erken ödeme ve yeniden yapılandırma işlemini gerçekleştirir.
        /// </summary>
        [HttpPost("{loanId}/partial-repayment")]
        public async Task<IActionResult> ProcessPartialRepayment(
            Guid loanId,
            [FromBody] PartialEarlyRepaymentRequestDto requestDto)
        {
            if (loanId == Guid.Empty)
                return BadRequest("Geçerli bir Kredi ID'si girmelisiniz.");

            try
            {
                var response = await _partialRepaymentService.ProcessPartialRepaymentAsync(requestDto);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}