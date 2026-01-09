using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.User;
using Eshop.Server.Application.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controllers
{
    /// <summary>
    /// Operazioni di amministrazione sugli users.
    /// Accesso riservato agli admin.
    /// </summary>
    [ApiController]
    [Route("api/admin/users")]
    [Authorize(Policy = "OnlyAdmin")]
    public class AdminUsersController : ControllerBase
    {
        private readonly UserService _userService;

        public AdminUsersController(UserService userService)
        {
            _userService = userService;
        }

        // =========================================
        // GET: api/admin/users
        // lista completa
        // =========================================
        /// <summary>
        /// Restituisce l'elenco completo degli users.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserReadDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UserReadDTO>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // =========================================
        // GET: api/admin/users/paged?search=...&page=1&pageSize=20
        // lista paginata + filtro
        // =========================================
        /// <summary>
        /// Restituisce gli users con ricerca e paginazione.
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _userService.GetAllAsync(search, page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                total,
                items
            });
        }

        // =========================================
        // GET: api/admin/users/{id}
        // dettaglio
        // =========================================
        /// <summary>
        /// Restituisce i dati di un user specifico.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserReadDTO>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound(new { message = $"User con id {id} non trovato." });

            return Ok(user);
        }

        // =========================================
        // PUT: api/admin/users/{id}
        // modifica user
        // =========================================
        /// <summary>
        /// Aggiorna i dati di un user (usato dall'amministratore).
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _userService.UpdateUserAsync(id, dto);
            if (!ok)
                return NotFound(new { message = $"User con id {id} non trovato." });

            return Ok(new { message = "User aggiornato." });
        }

        // =========================================
        // DELETE: api/admin/users/{id}
        // elimina user
        // =========================================
        /// <summary>
        /// Elimina un user dal sistema.
        /// Il service non elimina users admin.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _userService.DeleteUserAsync(id);
            if (!ok)
                return NotFound(new { message = $"User con id {id} non trovato oppure non eliminabile." });

            return Ok(new { message = "User eliminato." });
        }
    }
}
