using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Services.IServices;
using System.Security.Claims;

namespace OnlineEvaluation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AcademicMapController : ControllerBase
    {
        private readonly IAcademicMapService _service;

        public AcademicMapController(IAcademicMapService service)
        {
            _service = service;
        }

        [HttpGet("init-data")]
        public async Task<IActionResult> GetInitData()
        {
            var data = await _service.GetInitDataAsync();
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var results = await _service.GetAllAsync(page, pageSize);
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAcademicMapDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _service.CreateAsync(dto, userId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
               
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAcademicMapDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var success = await _service.UpdateAsync(id, dto, userId);
                if (!success) return NotFound();
                return Ok(new { message = "Academic Map updated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool success;

            if (hardDelete)
            {
                success = await _service.HardDeleteAsync(id);
            }
            else
            {
                success = await _service.SoftDeleteAsync(id, userId);
            }

            if (!success) return NotFound();
            return NoContent();
        }
    }
}
