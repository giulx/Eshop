using Microsoft.AspNetCore.Mvc;
using ApiServer.Application.Interfaces;
using ApiServer.DTOs;

namespace ApiServer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UtenteCreateDto dto)
        {
            var result = await _authenticationService.RegisterAsync(dto.Nome, dto.Cognome, dto.Email, dto.Password);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UtenteLoginDto dto)
        {
            var result = await _authenticationService.LoginAsync(dto.Email, dto.Password);
            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(new { token = result.Token });
        }
    }
}
