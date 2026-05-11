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

        // Controller sadece Service'i bilir, Domain veya DB'yi bilmez.
        public LoansController(ILoanApplicationService loanService)
        {
            _loanService = loanService;
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
    }
}