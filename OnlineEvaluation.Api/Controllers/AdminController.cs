using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OnlineEvaluation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("health")]
        public IActionResult Health() => Ok(new { message = "Admin are healthy" });



        [HttpGet("debug-token")]
        [Authorize] // If this works, the token is valid. If 403 follows, roles are wrong.
        public IActionResult DebugToken()
        {
            var identity = User.Identity as ClaimsIdentity;
            return Ok(new
            {
                Name = User.Identity?.Name, // Should show your GUID
                IsAdmin = User.IsInRole("Admin"), // If False, this is the 403 cause
                RoleClaimsFound = User.Claims
                    .Where(c => c.Type == "role" || c.Type == ClaimTypes.Role)
                    .Select(c => new { c.Type, c.Value })
            });
        }
    }





}
