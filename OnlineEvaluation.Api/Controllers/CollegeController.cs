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
    public class CollegeController : ControllerBase
    {
        private readonly ICollegeService _collegeService;

        public CollegeController(ICollegeService collegeService)
        {
            _collegeService = collegeService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CollegeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var colleges = await _collegeService.GetAllAsync(page, pageSize);
            return Ok(colleges);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CollegeDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var college = await _collegeService.GetByIdAsync(id);
            return college != null ? Ok(college) : NotFound();
        }

        [HttpGet("university/{universityCode}")]
        [ProducesResponseType(typeof(IEnumerable<CollegeDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUniversity(string universityCode)
        {
            var colleges = await _collegeService.GetByUniversityAsync(universityCode);
            return Ok(colleges);
        }

        [HttpPost]
        [ProducesResponseType(typeof(CollegeDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCollegeDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _collegeService.CreateAsync(dto, userId);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCollegeDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _collegeService.UpdateAsync(id, dto, userId);

            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (hardDelete)
            {
                var hardSuccess = await _collegeService.HardDeleteAsync(id);
                return hardSuccess ? NoContent() : NotFound();
            }

            var softSuccess = await _collegeService.SoftDeleteAsync(id, userId);
            return softSuccess ? NoContent() : NotFound();
        }

    }
}
