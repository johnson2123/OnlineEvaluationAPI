using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Models.Entities;
using OnlineEvaluation.Api.Services.IServices;
using System.Security.Claims;

namespace OnlineEvaluation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDto>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize parameter variables must be greater than zero.");
            }

            var departments = await _departmentService.GetAllAsync(page, pageSize);
            return Ok(departments);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentDto>> GetById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
            {
                return NotFound($"Department with ID {id} was not found.");
            }

            return Ok(department);
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<DepartmentDto>> GetByCode(string code)
        {
            var department = await _departmentService.GetByCodeAsync(code);
            if (department == null)
            {
                return NotFound($"Department with code '{code}' was not found.");
            }

            return Ok(department);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentDto>> Create([FromBody] CreateDepartmentDto dto)
        {
            try
            {
                var actorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var createdDepartment = await _departmentService.CreateAsync(dto, actorUserId);

                return CreatedAtAction(nameof(GetById), new { id = createdDepartment.Id }, createdDepartment);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDepartmentDto dto)
        {
            try
            {
                var actorUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var success = await _departmentService.UpdateAsync(id, dto, actorUserId);

                if (!success)
                {
                    return NotFound($"Department with ID {id} was not found.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (hardDelete)
            {
                var hardSuccess = await _departmentService.HardDeleteAsync(id);
                return hardSuccess ? NoContent() : NotFound();
            }

            var softSuccess = await _departmentService.SoftDeleteAsync(id, userId);
            return softSuccess ? NoContent() : NotFound();
        }
    }
}
