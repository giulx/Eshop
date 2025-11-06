using System.Threading.Tasks;
using Eshop.Server.Api.Extensions; // per User.GetUserId()
using Eshop.Server.Applicazione.DTOs.Utente;
using Eshop.Server.Applicazione.ServiziApplicativi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtentiController : ControllerBase
    {
        private readonly UtenteService _utenteService;

        public UtentiController(UtenteService utenteService)
        {
            _utenteService = utenteService;
        }

        // =====================================================
        // PUBBLICO: registrazione nuovo utente
        // POST: api/utenti/register
        // =====================================================
        /// <summary>
        /// Registra un nuovo utente nel sistema.
        /// Accesso pubblico, non richiede autenticazione.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] UtenteCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var creato = await _utenteService.CreaUtenteAsync(dto);
            if (!creato)
                return Conflict(new { message = "Email già registrata." });

            return StatusCode(StatusCodes.Status201Created, new { message = "Registrazione completata con successo." });
        }

        // =====================================================
        // CLIENTE: visualizza i propri dati
        // GET: api/utenti/me
        // =====================================================
        /// <summary>
        /// Restituisce i dati dell'utente loggato (cliente).
        /// </summary>
        [HttpGet("me")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(typeof(UtenteReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UtenteReadDTO>> GetMe()
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var utente = await _utenteService.GetByIdAsync(userId.Value);
            if (utente == null)
                return NotFound(new { message = "Utente non trovato." });

            return Ok(utente);
        }

        // =====================================================
        // CLIENTE: aggiorna i propri dati
        // PUT: api/utenti/me
        // =====================================================
        /// <summary>
        /// Aggiorna i dati personali dell'utente loggato.
        /// </summary>
        [HttpPut("me")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateMe([FromBody] UtenteUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var ok = await _utenteService.AggiornaUtenteAsync(userId.Value, dto);
            if (!ok)
                return BadRequest(new { message = "Impossibile aggiornare l’utente." });

            return Ok(new { message = "Profilo aggiornato correttamente." });
        }

        // =====================================================
        // CLIENTE: elimina il proprio account
        // DELETE: api/utenti/me
        // =====================================================
        /// <summary>
        /// Elimina definitivamente l'account dell'utente loggato.
        /// </summary>
        [HttpDelete("me")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteMe()
        {
            var userId = User.GetUserId();
            if (userId is null)
                return Unauthorized();

            var ok = await _utenteService.EliminaUtenteAsync(userId.Value);
            if (!ok)
                return NotFound(new { message = "Utente non trovato o non eliminabile." });

            return Ok(new { message = "Account eliminato con successo." });
        }
    }
}
