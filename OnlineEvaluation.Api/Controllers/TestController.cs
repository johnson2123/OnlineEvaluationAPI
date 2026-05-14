using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace OnlineEvaluation.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("debug")]
        [Authorize] // No Role requirement here so it won't 403
        public IActionResult DebugToken()
        {
            var identity = User.Identity as ClaimsIdentity;
            return Ok(new
            {
                AuthenticationType = identity?.AuthenticationType,
                IsAuthenticated = identity?.IsAuthenticated,
                // THIS IS THE KEY: If this is false, the 403 is confirmed
                IsAdminInRole = User.IsInRole("Admin"),
                // This will show exactly what .NET renamed your claims to
                Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            });
        }
    }
}
