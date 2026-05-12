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
    [Authorize(Roles ="Admin")]
    public class UniversityController : ControllerBase
    {
        private readonly IUniversityService _universityService;

        public UniversityController(IUniversityService universityService)
        {
            _universityService = universityService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            return Ok(await _universityService.GetAllAsync(page, pageSize));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var university = await _universityService.GetByIdAsync(id);
            return university == null ? NotFound() : Ok(university);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUniversityDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _universityService.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUniversityDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _universityService.UpdateAsync(id, dto, userId);
            return success ? NoContent() : NotFound();
        }

        //Soft and Hard Delete
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (hardDelete)
            {
                var hardSuccess = await _universityService.HardDeleteAsync(id);
                return hardSuccess ? NoContent() : NotFound();
            }

            var softSuccess = await _universityService.SoftDeleteAsync(id, userId);
            return softSuccess ? NoContent() : NotFound();
        }
    }
}
