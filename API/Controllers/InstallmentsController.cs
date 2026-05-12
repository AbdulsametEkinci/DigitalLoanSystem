using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigitalLoanSystem.Application.Interfaces;

namespace DigitalLoanSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InstallmentsController : ControllerBase
    {
        private readonly IInstallmentApplicationService _installmentService;

        public InstallmentsController(IInstallmentApplicationService installmentService)
        {
            _installmentService = installmentService;
        }

        /// <summary>
        /// UC-3: Bir krediye ait taksit planını listeler
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid loanId)
        {
            if (loanId == Guid.Empty)
                return BadRequest("Geçerli bir Kredi ID'si girmelisiniz.");

            var installments = await _installmentService.GetInstallmentsByLoanIdAsync(loanId);
            return Ok(installments);
        }
    }
}