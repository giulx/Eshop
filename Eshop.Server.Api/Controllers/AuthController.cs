using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Auth;
using Eshop.Server.Application.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controllers
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

            // login fallito → 401 con il DTO completo (Success=false, Message, ErrorCode, ecc.)
            if (!result.Success)
            {
                return Unauthorized(result);
            }

            // login OK → 200 con il DTO completo
            return Ok(result);
        }
    }
}
