using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<StudentOnboardingController> _logger;
        private readonly IHostEnvironment _env;

        private const int MaxBulkPayloadSize = 1000;

        public StudentOnboardingController(
            IStudentOnboardingService onboardingService,
            ILogger<StudentOnboardingController> logger,
            IHostEnvironment env)
        {
            _onboardingService = onboardingService ?? throw new ArgumentNullException(nameof(onboardingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env;
        }

        [HttpPost("single")]
        public async Task<IActionResult> RegisterSingle([FromBody] StudentRegistrationDto dto)
        {
            if (dto == null) return BadRequest(new { Message = "Payload cannot be null." });

            string actorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";

            try
            {
                var createdStudent = await _onboardingService.RegisterSingleStudentAsync(dto, actorUserId);
                return CreatedAtAction(nameof(RegisterSingle), new { id = createdStudent.Id }, createdStudent);
            }
            catch (ArgumentException ex) { return BadRequest(new { Message = ex.Message }); }
            catch (InvalidOperationException ex) { return UnprocessableEntity(new { Message = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { Message = ex.Message }); }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration failure. Actor {Actor}, RegNo {RegNo}", actorUserId, dto?.RegistrationNumber);
                return StatusCode(500, new
                {
                    Message = "An internal error occurred.",
                    Detail = _env.IsDevelopment() ? ex.Message : null
                });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> RegisterBulk([FromBody] List<StudentRegistrationDto> dtos)
        {
            if (dtos == null || !dtos.Any()) return BadRequest(new { Message = "Payload empty." });
            if (dtos.Count > MaxBulkPayloadSize) return BadRequest(new { Message = "Bulk limit exceeded." });

            string actorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "SYSTEM";

            try
            {
                var result = await _onboardingService.RegisterBulkStudentsAsync(dtos, actorUserId);

                if (result.SuccessfullyProcessedCount == 0 && result.Errors.Any())
                {
                    return UnprocessableEntity(new { Message = "All data chunks failed processing.", Result = result });
                }

                if (result.Errors.Any())
                {
                    return StatusCode(207, result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk ingestion failure. Actor {Actor}, Count {Count}", actorUserId, dtos?.Count);
                return StatusCode(500, new
                {
                    Message = "Critical system isolation breakdown.",
                    Detail = _env.IsDevelopment() ? ex.Message : null
                });
            }
        }
    }
}
