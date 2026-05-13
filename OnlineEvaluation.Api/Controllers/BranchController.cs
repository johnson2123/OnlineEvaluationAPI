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
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _branchService.GetAllAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _branchService.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpGet("code/{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _branchService.GetByCodeAsync(code);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBranchDto dto)
        {
            var existing = await _branchService.GetByCodeAsync(dto.Code);
            if (existing != null) return BadRequest("Branch code already exists.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _branchService.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBranchDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _branchService.UpdateAsync(id, dto, userId);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (hardDelete)
            {
                var hardSuccess = await _branchService.HardDeleteAsync(id);
                return hardSuccess ? NoContent() : NotFound();
            }

            var softSuccess = await _branchService.SoftDeleteAsync(id, userId);
            return softSuccess ? NoContent() : NotFound();
        }
    }
}
