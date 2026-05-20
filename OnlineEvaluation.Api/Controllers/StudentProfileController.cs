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
    [Authorize(Roles = "Student")]
    public class StudentProfileController : ControllerBase
    {
        private readonly IStudentProfileService _profileService;

        public StudentProfileController(IStudentProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StudentProfileDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User identity claim session context could not be resolved.");
            }

            var profile = await _profileService.GetProfileByUserIdAsync(userId);

            if (profile == null)
            {
                return NotFound("No student tracking record profile maps to this identity account.");
            }

            return Ok(profile);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateStudentProfileDto dto)
        {  
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User identity claim session context could not be resolved.");
            }

            try
            {
                var result = await _profileService.UpdateProfileAsync(userId, dto);

                if (!result)
                {
                    return BadRequest("No changes were applied to the profile registry database tracking entry.");
                }

                return Ok(new { message = "Your personal profile particulars have been modified successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An error occurred while tracking changes to your security profile record.");
            }
        }
    }
}
