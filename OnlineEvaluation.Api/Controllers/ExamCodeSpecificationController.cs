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
    [Authorize(Roles = "Admin,Controller")]
    public class ExamCodeSpecificationController : ControllerBase
    {
        private readonly IExamCodeSpecificationService _specService;

        public ExamCodeSpecificationController(IExamCodeSpecificationService specService)
        {
            _specService = specService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ExamSpecDto>))]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var results = await _specService.GetAllAsync(page, pageSize);
            return Ok(results);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExamSpecDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _specService.GetByIdAsync(id);
            if (result == null)
            {
                return NotFound(new { message = $"Exam specification with ID {id} was not found." });
            }

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ExamSpecDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateExamSpecDto dto)
        {

            try
            {
                var actorUserId = GetCurrentUserId();
                var result = await _specService.CreateAsync(dto, actorUserId);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                // Catches our business rules constraint 
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                // Catches missing parent master mappings 
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExamSpecDto dto)
        {
            try
            {
                var actorUserId = GetCurrentUserId();
                bool isUpdated = await _specService.UpdateAsync(id, dto, actorUserId);

                if (!isUpdated) return BadRequest(new { message = "No records were modified during the update transaction." });

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            try
            {
                if (hardDelete)
                {
                   
                    if (!User.IsInRole("Admin"))
                    {
                        return Forbid();
                    }

                    bool hardSuccess = await _specService.HardDeleteAsync(id);
                    return hardSuccess ? NoContent() : NotFound(new { message = $"Specification with ID {id} not found." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                bool softSuccess = await _specService.SoftDeleteAsync(id, userId);
                return softSuccess ? NoContent() : NotFound(new { message = $"Specification with ID {id} not found." });
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

        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("uid");
        }
    }
}
