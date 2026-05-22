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
    public class CollegeDepartmentController : ControllerBase
    {
        private readonly ICollegeDepartmentService _service;

        public CollegeDepartmentController(ICollegeDepartmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CollegeDepartmentDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CollegeDepartmentDto>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = $"Mapping with ID {id} was not found." });
            }
            return Ok(result);
        }

        [HttpGet("college/{collegeId:int}")]
        public async Task<ActionResult<IEnumerable<CollegeDepartmentDto>>> GetByCollege(int collegeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetByCollegeAsync(collegeId, page, pageSize);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CollegeDepartmentDto>> Create([FromBody] CreateCollegeDepartmentDto dto)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
                var createdMapping = await _service.MapDepartmentAsync(dto, userId);

                return CreatedAtAction(nameof(GetById), new { id = createdMapping.Id }, createdMapping);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var success = await _service.RemoveMappingAsync(id, userId, hardDelete);

            if (!success)
            {
                return NotFound(new { message = $"Mapping with ID {id} was not found." });
            }
            return NoContent();
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateCollegeDepartmentStatusDto dto)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

            var success = await _service.UpdateStatusAsync(id, dto.IsActive, userId);

            if (!success)
            {
                return NotFound(new { message = $"Mapping with ID {id} was not found." });
            }
            return NoContent();
        }
    }
}
