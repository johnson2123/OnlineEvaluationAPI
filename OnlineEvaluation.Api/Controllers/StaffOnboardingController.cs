using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OnlineEvaluation.Api.Models.DTO;
using OnlineEvaluation.Api.Services.IServices;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OnlineEvaluation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class StaffOnboardingController : ControllerBase
    {
        private readonly IStaffOnboardingService _onboardingService;
        private readonly ILogger<StaffOnboardingController> _logger;

        public StaffOnboardingController(
            IStaffOnboardingService onboardingService,
            ILogger<StaffOnboardingController> logger)
        {
            _onboardingService = onboardingService;
            _logger = logger;
        }

        [HttpPost("single")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StaffDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterSingle([FromBody] StaffRegistrationDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { ErrorMessage = "Payload context body cannot be empty." });
            }

            // Single registration requires immediate model state rejection
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string actorUserId = GetActorUserId();

                var processedStaffRecord = await _onboardingService.RegisterSingleStaffAsync(dto, actorUserId);

                return Created($"/api/staff/{processedStaffRecord.Id}", processedStaffRecord);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { ErrorMessage = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A system anomaly occurred during single ingestion execution.");

#if DEBUG
                return StatusCode(500, new
                {
                    ErrorMessage = "A system anomaly occurred during single ingestion execution.",
                    Details = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
#else
                return StatusCode(500, new { ErrorMessage = "A system anomaly occurred during single ingestion execution." });
#endif
            }
        }

        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BulkOperationResultDto<BulkRowErrorDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegisterBulk([FromBody] List<StaffRegistrationDto> dtos)
        {
            // Null or structurally unreadable request bodies fail early
            if (dtos == null || dtos.Count == 0)
            {
                return BadRequest(new { ErrorMessage = "Bulk collection dataset cannot be empty or structurally unreadable." });
            }

            try
            {
                string actorUserId = GetActorUserId();

                // Note: Let the service process the items so that individual row errors 
                // are gracefully appended to your bulk operation summary collection report.
                var bulkExecutionSummary = await _onboardingService.RegisterBulkStaffAsync(dtos, actorUserId);

                return Ok(bulkExecutionSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A critical failure disrupted the bulk batch processing engine.");

#if DEBUG
                return StatusCode(500, new
                {
                    ErrorMessage = "A critical failure disrupted the bulk batch processing engine.",
                    Details = ex.Message,
                    InnerError = ex.InnerException?.Message
                });
#else
                return StatusCode(500, new { ErrorMessage = "A critical failure disrupted the bulk batch processing engine." });
#endif
            }
        }

        private string GetActorUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("uid")?.Value
                   ?? "SYSTEM";
        }
    }
}