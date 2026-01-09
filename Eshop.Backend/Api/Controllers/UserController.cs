using System.Threading.Tasks;
using Eshop.Server.Api.Extensions; // per User.GetUserId()
using Eshop.Server.Application.DTOs.User;
using Eshop.Server.Application.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        // =====================================================
        // PUBBLICO: registrazione new user
        // POST: api/users/register
        // =====================================================
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var creato = await _userService.CreateUserAsync(dto);
            if (!creato)
                return Conflict(new { message = "Email già registrata." });

            return StatusCode(StatusCodes.Status201Created, new { message = "Registrazione completata con successo." });
        }

        // =====================================================
        // CLIENTE: visualizza i propri dati
        // GET: api/users/me
        // =====================================================
        [HttpGet("me")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(typeof(UserReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserReadDTO>> GetMe()
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
                return NotFound(new { message = "User non trovato." });

            return Ok(user);
        }

        // =====================================================
        // CLIENTE: aggiorna i propri dati
        // PUT: api/users/me
        // =====================================================
        [HttpPut("me")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMe([FromBody] UserUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var ok = await _userService.UpdateUserAsync(userId.Value, dto);
            if (!ok)
                return BadRequest(new { message = "Impossibile aggiornare l’user." });

            return Ok(new { message = "Profilo aggiornato correttamente." });
        }

        // =====================================================
        // CLIENTE: elimina il proprio account
        // DELETE: api/users/me
        // =====================================================
        [HttpDelete("me")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMe()
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var ok = await _userService.DeleteUserAsync(userId.Value);
            if (!ok)
                return NotFound(new { message = "User non trovato o non eliminabile." });

            return Ok(new { message = "Account eliminato con successo." });
        }

        // =====================================================
        // ADMIN: lista utenti (filtro + paginazione)
        // GET: api/users?search=...&page=1&pageSize=20
        // =====================================================
        [HttpGet]
        [Authorize(Policy = "OnlyAdmin")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _userService.GetAllAsync(search, page, pageSize);
            return Ok(new { page, pageSize, total, items });
        }

        // =====================================================
        // ADMIN: dettaglio utente
        // GET: api/users/5
        // =====================================================
        [HttpGet("{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User non trovato." });

            return Ok(user);
        }

        // =====================================================
        // ADMIN: aggiorna utente (tutto tranne email)
        // PUT: api/users/5
        // =====================================================
        [HttpPut("{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        public async Task<IActionResult> AdminUpdate(int id, [FromBody] UserUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _userService.UpdateUserAsync(id, dto);
            if (!ok)
                return NotFound(new { message = "User non trovato o non aggiornabile." });

            return Ok(new { message = "User aggiornato correttamente." });
        }

        // =====================================================
        // ADMIN: elimina utente (non elimina admin)
        // DELETE: api/users/5
        // =====================================================
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        public async Task<IActionResult> AdminDelete(int id)
        {
            var ok = await _userService.DeleteUserAsync(id);
            if (!ok)
                return BadRequest(new { message = "User non trovato o non eliminabile." });

            return Ok(new { message = "User eliminato con successo." });
        }
    }
}
