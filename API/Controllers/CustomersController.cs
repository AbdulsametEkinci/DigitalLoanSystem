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

        [HttpGet]
        public async Task<IActionResult> GetAllCustomers()
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync();
                return Ok(customers);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = "Müşteri listesi getirilirken bir hata oluştu: " + ex.Message });
            }
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomer(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçerli bir Müşteri ID'si girmelisiniz.");

            try
            {
                var customer = await _customerService.GetCustomerAsync(id);
                return Ok(customer);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = "Müşteri bilgileri getirilirken bir hata oluştu: " + ex.Message });
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
        /// Müşteri bilgilerini güncelleme
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] UpdateCustomerDto requestDto)
        {
            if (id == Guid.Empty)
                return BadRequest("Geçerli bir Müşteri ID'si girmelisiniz.");

            try
            {
                var updatedCustomer = await _customerService.UpdateCustomerAsync(id, requestDto);
                return Ok(updatedCustomer);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Error = ex.Message });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { Error = "Müşteri güncellenirken bir hata oluştu: " + ex.Message });
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