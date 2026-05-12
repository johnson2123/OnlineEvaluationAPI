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
    public class StudyProgramController : ControllerBase
    {
        private readonly IStudyProgramService _studyProgramService;

        public StudyProgramController(IStudyProgramService studyProgramService)
        {
            _studyProgramService = studyProgramService;
        }

        // Centralized User ID retrieval
        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System_Admin";

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StudyProgramDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var programs = await _studyProgramService.GetAllAsync(page, pageSize);
            return Ok(programs);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StudyProgramDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var program = await _studyProgramService.GetByIdAsync(id);
            return program != null ? Ok(program) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(StudyProgramDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateStudyProgramDto dto)
        {
            var result = await _studyProgramService.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStudyProgramDto dto)
        {
            var success = await _studyProgramService.UpdateAsync(dto, CurrentUserId);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, [FromQuery] bool hardDelete = false)
        {
            if (hardDelete)
            {
                var hardSuccess = await _studyProgramService.HardDeleteAsync(id);
                return hardSuccess ? NoContent() : NotFound();
            }

            // Using the helper property
            var softSuccess = await _studyProgramService.SoftDeleteAsync(id, CurrentUserId);
            return softSuccess ? NoContent() : NotFound();
        }

        [HttpPost("{id:int}/restore")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Restore(int id)
        {
            var success = await _studyProgramService.RestoreAsync(id);
            return success ? NoContent() : NotFound();
        }


    }
}
