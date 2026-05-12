using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DigitalLoanSystem.Application.DTOs;
using DigitalLoanSystem.Application.Interfaces;

namespace DigitalLoanSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerApplicationService _customerService;

        public CustomersController(ICustomerApplicationService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto requestDto)
        {
            try
            {
                var response = await _customerService.CreateCustomerAsync(requestDto);
                return Created($"/api/customers/{response.Id}", response);
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
        /// <summary>
        /// UC-5: Müşterinin Borç ve Finansal Özetini getirir.
        /// </summary>
        [HttpGet("{id}/summary")] // RESTful URL: GET /api/customers/{id}/summary
        public async Task<IActionResult> GetCustomerSummary(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçerli bir Müşteri ID'si girmelisiniz.");

            try
            {
                var summary = await _customerService.GetCustomerSummaryAsync(id);
                return Ok(summary); // 200 OK
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = "Özet hesaplanırken bir hata oluştu: " + ex.Message });
            }
        }

        /// <summary>
        /// Müşteri silme işlemi
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçerli bir Müşteri ID'si girmelisiniz.");

            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                return NoContent(); // 204 No Content
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Error = ex.Message }); // 404 Not Found
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = "Müşteri silinirken bir hata oluştu: " + ex.Message });
            }
        }
    }
}