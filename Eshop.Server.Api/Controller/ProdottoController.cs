using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Prodotto;
using Eshop.Server.Application.ServiziApplicativi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eshop.Server.Api.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProdottoController : ControllerBase
    {
        private readonly ProdottoService _prodottoService;

        public ProdottoController(ProdottoService prodottoService)
        {
            _prodottoService = prodottoService;
        }

        // ================================
        // GET: api/v1/prodotto?search=...&page=1&pageSize=20
        // ================================
        /// <summary>
        /// Restituisce il catalogo prodotti con ricerca e paginazione (pubblico).
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Lista prodotti (pubblico) con filtro e paginazione")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var (items, total) = await _prodottoService.GetAllAsync(search, page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                total,
                items
            });
        }

        // ================================
        // GET: api/v1/prodotto/5
        // ================================
        /// <summary>
        /// Restituisce un singolo prodotto per Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Dettaglio di un prodotto (pubblico)")]
        [ProducesResponseType(typeof(ProdottoReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProdottoReadDTO>> GetById(int id)
        {
            var prodotto = await _prodottoService.GetByIdAsync(id);
            if (prodotto == null)
                return NotFound(new { message = $"Prodotto con id {id} non trovato." });

            return Ok(prodotto);
        }

        // ================================
        // POST: api/v1/prodotto
        // ================================
        /// <summary>
        /// Crea un nuovo prodotto nel catalogo (solo admin).
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "OnlyAdmin")]
        [SwaggerOperation(Summary = "Crea un nuovo prodotto (solo admin)")]
        [ProducesResponseType(typeof(ProdottoReadDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] ProdottoCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var creato = await _prodottoService.CreaProdottoAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = creato.Id }, creato);
        }

        // ================================
        // PUT: api/v1/prodotto/5
        // ================================
        /// <summary>
        /// Aggiorna un prodotto esistente (solo admin).
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        [SwaggerOperation(Summary = "Aggiorna un prodotto (solo admin)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProdottoUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var aggiornato = await _prodottoService.AggiornaProdottoAsync(id, dto);

            if (!aggiornato)
                return NotFound(new { message = $"Prodotto con id {id} non trovato." });

            return NoContent();
        }

        // ================================
        // DELETE: api/v1/prodotto/5
        // ================================
        /// <summary>
        /// Elimina un prodotto specifico (solo admin).
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        [SwaggerOperation(Summary = "Elimina un prodotto (solo admin)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteById(int id)
        {
            var eliminato = await _prodottoService.EliminaProdottoAsync(id);

            if (!eliminato)
                return NotFound(new { message = $"Prodotto con id {id} non trovato." });

            return Ok(new { message = $"Prodotto con id {id} eliminato con successo." });
        }

        // ================================
        // DELETE: api/v1/prodotto
        // ================================
        /// <summary>
        /// Elimina tutti i prodotti (solo admin, uso test).
        /// </summary>
        [HttpDelete]
        [Authorize(Policy = "OnlyAdmin")]
        [SwaggerOperation(Summary = "Elimina TUTTI i prodotti (solo admin)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteAll()
        {
            await _prodottoService.EliminaTuttiAsync();
            return Ok(new { message = "Tutti i prodotti sono stati eliminati." });
        }
    }
}
