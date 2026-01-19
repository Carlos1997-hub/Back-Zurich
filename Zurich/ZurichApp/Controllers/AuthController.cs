using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZurichApp.Api.Dtos.Auth;
using ZurichApp.Api.Services.Interfaces;

namespace ZurichApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            var res = await _auth.LoginAsync(request);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { ok = true });
        }
    }
}
