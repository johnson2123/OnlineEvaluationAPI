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
    public class StudentOnboardingController : ControllerBase
    {
        private readonly IStudentOnboardingService _onboardingService;

        public StudentOnboardingController(IStudentOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [HttpPost("single")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StudentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegisterSingle([FromBody] StudentRegistrationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {     
                string actorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";

                var createdStudent = await _onboardingService.RegisterSingleStudentAsync(dto, actorUserId);

                return Created($"/api/students/{createdStudent.Id}", createdStudent);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BulkOperationResultDto<BulkRowErrorDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterBulk([FromBody] List<StudentRegistrationDto> dtos)
        {
            if (dtos == null || !dtos.Any())
            {
                return BadRequest(new { Message = "The ingestion payload dataset matrix cannot be empty." });
            }
       
            string actorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";

            var result = await _onboardingService.RegisterBulkStudentsAsync(dtos, actorUserId);

            return Ok(result);
        }
    }
}
