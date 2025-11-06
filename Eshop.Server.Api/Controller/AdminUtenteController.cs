using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Applicazione.DTOs.Utente;
using Eshop.Server.Applicazione.ServiziApplicativi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controllers
{
    /// <summary>
    /// Operazioni di amministrazione sugli utenti.
    /// Accesso riservato agli admin.
    /// </summary>
    [ApiController]
    [Route("api/admin/utenti")]
    [Authorize(Policy = "OnlyAdmin")]
    public class AdminUtentiController : ControllerBase
    {
        private readonly UtenteService _utenteService;

        public AdminUtentiController(UtenteService utenteService)
        {
            _utenteService = utenteService;
        }

        // =========================================
        // GET: api/admin/utenti
        // lista completa
        // =========================================
        /// <summary>
        /// Restituisce l'elenco completo degli utenti.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UtenteReadDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UtenteReadDTO>>> GetAll()
        {
            var utenti = await _utenteService.GetAllAsync();
            return Ok(utenti);
        }

        // =========================================
        // GET: api/admin/utenti/paged?search=...&page=1&pageSize=20
        // lista paginata + filtro
        // =========================================
        /// <summary>
        /// Restituisce gli utenti con ricerca e paginazione.
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaged(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var (items, total) = await _utenteService.GetAllAsync(search, page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                total,
                items
            });
        }

        // =========================================
        // GET: api/admin/utenti/{id}
        // dettaglio
        // =========================================
        /// <summary>
        /// Restituisce i dati di un utente specifico.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UtenteReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UtenteReadDTO>> GetById(int id)
        {
            var utente = await _utenteService.GetByIdAsync(id);
            if (utente == null)
                return NotFound(new { message = $"Utente con id {id} non trovato." });

            return Ok(utente);
        }

        // =========================================
        // PUT: api/admin/utenti/{id}
        // modifica utente
        // =========================================
        /// <summary>
        /// Aggiorna i dati di un utente (usato dall'amministratore).
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UtenteUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ok = await _utenteService.AggiornaUtenteAsync(id, dto);
            if (!ok)
                return NotFound(new { message = $"Utente con id {id} non trovato." });

            return Ok(new { message = "Utente aggiornato." });
        }

        // =========================================
        // DELETE: api/admin/utenti/{id}
        // elimina utente
        // =========================================
        /// <summary>
        /// Elimina un utente dal sistema.
        /// Il service non elimina utenti admin.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _utenteService.EliminaUtenteAsync(id);
            if (!ok)
                return NotFound(new { message = $"Utente con id {id} non trovato oppure non eliminabile." });

            return Ok(new { message = "Utente eliminato." });
        }
    }
}
