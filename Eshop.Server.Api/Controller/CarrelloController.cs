using Eshop.Server.Applicazione.DTOs.Carrello;
using Eshop.Server.Applicazione.ServiziApplicativi;
using Eshop.Server.Api.Extensions; // per User.GetUserId()
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eshop.Server.Api.Controllers
{
    [Authorize(Policy = "OnlyCliente")]
    [ApiController]
    [Route("api/[controller]")]
    public class CarrelloController : ControllerBase
    {
        private readonly CarrelloService _carrelloService;

        public CarrelloController(CarrelloService carrelloService)
        {
            _carrelloService = carrelloService;
        }

        // GET: api/carrello/mio
        [HttpGet("mio")]
        [SwaggerOperation(Summary = "Ottiene il carrello del cliente loggato")]
        [ProducesResponseType(typeof(CarrelloReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMioCarrello(CancellationToken ct)
        {
            var clienteId = User.GetUserId();
            if (clienteId is null)
                return Unauthorized();

            var carrello = await _carrelloService.GetByClienteIdAsync(clienteId.Value);
            if (carrello is null)
                return NotFound("Carrello non trovato.");

            return Ok(carrello);
        }

        // POST: api/carrello/mio/voce
        [HttpPost("mio/voce")]
        [SwaggerOperation(Summary = "Aggiunge o incrementa una voce nel carrello del cliente")]
        [ProducesResponseType(typeof(CarrelloReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AggiungiVoce([FromBody] AggiungiVoceCarrelloDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clienteId = User.GetUserId();
            if (clienteId is null)
                return Unauthorized();

            var ok = await _carrelloService.AggiungiVoceAsync(clienteId.Value, dto.ProdottoId, dto.Quantita);
            if (!ok)
                return BadRequest(new { message = "Impossibile aggiungere il prodotto al carrello." });

            var carrelloAggiornato = await _carrelloService.GetByClienteIdAsync(clienteId.Value);
            return Ok(carrelloAggiornato);
        }

        // PUT: api/carrello/mio/voce
        [HttpPut("mio/voce")]
        [SwaggerOperation(Summary = "Aggiorna la quantità di una voce nel carrello del cliente")]
        [ProducesResponseType(typeof(CarrelloReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CambiaQuantita([FromBody] CambiaQuantitaCarrelloDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var clienteId = User.GetUserId();
            if (clienteId is null)
                return Unauthorized();

            var ok = await _carrelloService.CambiaQuantitaAsync(clienteId.Value, dto.ProdottoId, dto.Quantita);
            if (!ok)
                return NotFound(new { message = "Carrello o voce non trovati." });

            var carrelloAggiornato = await _carrelloService.GetByClienteIdAsync(clienteId.Value);
            return Ok(carrelloAggiornato);
        }

        // DELETE: api/carrello/mio/voce
        [HttpDelete("mio/voce")]
        [SwaggerOperation(Summary = "Rimuove una voce dal carrello del cliente")]
        [ProducesResponseType(typeof(CarrelloReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RimuoviVoce([FromBody] RimuoviVoceCarrelloDTO dto, CancellationToken ct)
        {
            var clienteId = User.GetUserId();
            if (clienteId is null)
                return Unauthorized();

            var ok = await _carrelloService.RimuoviVoceAsync(clienteId.Value, dto.ProdottoId);
            if (!ok)
                return NotFound(new { message = "Carrello o voce non trovati." });

            var carrelloAggiornato = await _carrelloService.GetByClienteIdAsync(clienteId.Value);
            return Ok(carrelloAggiornato);
        }
    }
}
