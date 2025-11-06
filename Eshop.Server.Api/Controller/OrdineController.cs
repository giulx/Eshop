using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Api.Extensions; // User.GetUserId()
using Eshop.Server.Applicazione.DTOs.Ordine;
using Eshop.Server.Applicazione.ServiziApplicativi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdiniController : ControllerBase
    {
        private readonly OrdineService _ordineService;

        public OrdiniController(OrdineService ordineService)
        {
            _ordineService = ordineService;
        }

        // ======================================================
        // CLIENTE: preview dal carrello
        // ======================================================
        [HttpGet("preview")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(typeof(OrdinePreviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPreview()
        {
            var userId = User.GetUserId()!.Value;

            var preview = await _ordineService.PreparaOrdineDaCarrelloAsync(userId);
            if (preview is null)
                return NotFound(new { error = "Carrello non trovato.", code = "cart_not_found" });

            return Ok(preview);
        }

        // ======================================================
        // CLIENTE: conferma ordine
        // ======================================================
        [HttpPost("conferma")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(typeof(ConfermaOrdineResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Conferma()
        {
            var userId = User.GetUserId()!.Value;

            var result = await _ordineService.ConfermaOrdineDaCarrelloAsync(userId);

            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "stock_changed" => Conflict(new
                    {
                        error = "La disponibilità è cambiata. Rifai la preview.",
                        code = "stock_changed",
                        result.RigheNonOrdinate
                    }),
                    "payment_failed" => BadRequest(new
                    {
                        error = "Pagamento non riuscito.",
                        code = "payment_failed"
                    }),
                    "no_items_orderable" => BadRequest(new
                    {
                        error = "Nessuna riga ordinabile nel carrello.",
                        code = "no_items_orderable"
                    }),
                    "cart_not_found" => NotFound(new
                    {
                        error = "Carrello non trovato.",
                        code = "cart_not_found"
                    }),
                    _ => BadRequest(new
                    {
                        error = "Impossibile confermare l’ordine.",
                        code = result.ErrorCode ?? "unknown_error"
                    })
                };
            }

            return Ok(result);
        }

        // ======================================================
        // CLIENTE: i miei ordini
        // ======================================================
        [HttpGet("miei")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(typeof(IEnumerable<OrdineReadDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMiei()
        {
            var userId = User.GetUserId()!.Value;
            var ordini = await _ordineService.GetByClienteAsync(userId);
            return Ok(ordini);
        }

        // ======================================================
        // CLIENTE: dettaglio di un mio ordine
        // ======================================================
        [HttpGet("{id:int}")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(typeof(OrdineReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMioOrdine(int id)
        {
            var userId = User.GetUserId()!.Value;

            var ordine = await _ordineService.GetByIdAsync(id);
            if (ordine is null || ordine.ClienteId != userId)
                return NotFound(new { error = "Ordine non trovato.", code = "order_not_found" });

            return Ok(ordine);
        }

        // ======================================================
        // CLIENTE: annulla un mio ordine
        // ======================================================
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "OnlyCliente")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AnnullaMioOrdine(int id)
        {
            var userId = User.GetUserId()!.Value;

            var ok = await _ordineService.AnnullaOrdineDaClienteAsync(id, userId);
            if (!ok)
                return Conflict(new { error = "Ordine non tuo o non più annullabile.", code = "cannot_cancel" });

            return Ok(new { message = "Ordine annullato e rimborsato.", code = "cancelled" });
        }

        // ======================================================
        // ADMIN: tutti gli ordini
        // ======================================================
        [HttpGet]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(typeof(IEnumerable<OrdineReadDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var ordini = await _ordineService.GetAllAsync();
            return Ok(ordini);
        }

        // ======================================================
        // ADMIN: annulla ordine
        // ======================================================
        [HttpDelete("admin/{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AnnullaDaAdmin(int id)
        {
            var ok = await _ordineService.AnnullaOrdineDaAdminAsync(id);
            if (!ok)
                return NotFound(new { error = "Ordine non trovato.", code = "order_not_found" });

            return Ok(new { message = "Ordine annullato dall’amministratore.", code = "admin_cancelled" });
        }

        // ======================================================
        // ADMIN: eliminazione definitiva
        // ======================================================
        [HttpDelete("admin/{id:int}/hard")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EliminaDefinitivo(int id)
        {
            var ok = await _ordineService.EliminaOrdineDefinitivoAsync(id);
            if (!ok)
                return Conflict(new
                {
                    error = "Ordine non trovato o ancora nello stato pagato.",
                    code = "cannot_hard_delete"
                });

            return Ok(new { message = "Ordine eliminato definitivamente.", code = "deleted" });
        }

        // ======================================================
        // ADMIN: cambia stato (nuovi)
        // ======================================================

        /// <summary>
        /// (Admin) Imposta l’ordine in stato "In elaborazione".
        /// </summary>
        [HttpPut("admin/{id:int}/in-elaborazione")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MettiInElaborazione(int id)
        {
            var ok = await _ordineService.MettiInElaborazioneDaAdminAsync(id);
            if (!ok)
                return NotFound(new { error = "Ordine non trovato.", code = "order_not_found" });

            return Ok(new { message = "Ordine messo in elaborazione.", code = "set_in_elaborazione" });
        }

        /// <summary>
        /// (Admin) Imposta l’ordine in stato "Spedito".
        /// </summary>
        [HttpPut("admin/{id:int}/spedito")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MettiSpedito(int id)
        {
            var ok = await _ordineService.SegnaSpeditoDaAdminAsync(id);
            if (!ok)
                return NotFound(new { error = "Ordine non trovato.", code = "order_not_found" });

            return Ok(new { message = "Ordine segnato come spedito.", code = "set_spedito" });
        }
    }
}
