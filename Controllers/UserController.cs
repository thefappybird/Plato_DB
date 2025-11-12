using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plato_DB.Dtos.User;
using System.IdentityModel.Tokens.Jwt;
using Plato_DB.Services.AuthService;
using System.Security.Claims;

namespace Plato_DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisterUserDto>> Register([FromForm] RegisterUserDto request)
        {
            var response = await authService.RegisterAsync(request);
            if (response.Passed == false)
            {
                Console.WriteLine(response.Message);
                return BadRequest(response.Message);
            }

            return Ok(response.Message);
        }
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromForm] LoginUserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result == null)
            {
                return BadRequest("Invalid username/email or password");
            }
            return Ok(result);
        }
        [Authorize]
        [HttpGet("auth")]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Try common claim names in a safe order
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst(JwtRegisteredClaimNames.Sub)
                          ?? User.FindFirst("sub");

            if (idClaim == null)
                return Unauthorized();

            if (!Guid.TryParse(idClaim.Value, out var userId))
                return Unauthorized();

            var userProfile = await authService.FetchUser(userId);
            if (userProfile == null)
                return NotFound();

            return Ok(userProfile);
        }
    }
}
