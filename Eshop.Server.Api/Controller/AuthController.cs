using System.Threading.Tasks;
using Eshop.Server.Applicazione.DTOs.Auth;
using Eshop.Server.Applicazione.ServiziApplicativi;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (!result.Success)
                return Unauthorized(new { result.Message });

            // qui ritorniamo il token
            return Ok(result);
        }
    }
}

