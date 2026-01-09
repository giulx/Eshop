using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Product;
using Eshop.Server.Application.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eshop.Server.Api.Controller
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // ================================
        // GET: api/v1/product?search=...&page=1&pageSize=20
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

            var (items, total) = await _productService.GetAllAsync(search, page, pageSize);

            return Ok(new
            {
                page,
                pageSize,
                total,
                items
            });
        }

        // ================================
        // GET: api/v1/product/5
        // ================================
        /// <summary>
        /// Restituisce un singolo product per Id.
        /// </summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Dettaglio di un product (pubblico)")]
        [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductReadDTO>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = $"Product con id {id} non trovato." });

            return Ok(product);
        }

        // ================================
        // POST: api/v1/product
        // ================================
        /// <summary>
        /// Crea un new product nel catalogo (solo admin).
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "OnlyAdmin")]
        [SwaggerOperation(Summary = "Crea un new product (solo admin)")]
        [ProducesResponseType(typeof(ProductReadDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Create([FromBody] ProductCreateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var creato = await _productService.CreateProductAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = creato.Id }, creato);
        }

        // ================================
        // PUT: api/v1/product/5
        // ================================
        /// <summary>
        /// Aggiorna un product existing (solo admin).
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        [SwaggerOperation(Summary = "Aggiorna un product (solo admin)")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var aggiornato = await _productService.UpdateProductAsync(id, dto);

            if (!aggiornato)
                return NotFound(new { message = $"Product con id {id} non trovato." });

            return NoContent();
        }

        // ================================
        // DELETE: api/v1/product/5
        // ================================
        /// <summary>
        /// Elimina un product specifico (solo admin).
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        [SwaggerOperation(Summary = "Elimina un product (solo admin)")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteById(int id)
        {
            var eliminato = await _productService.DeleteProductAsync(id);

            if (!eliminato)
                return NotFound(new { message = $"Product con id {id} non trovato." });

            return Ok(new { message = $"Product con id {id} eliminato con successo." });
        }

        // ================================
        // DELETE: api/v1/product
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
            await _productService.DeleteAllAsync();
            return Ok(new { message = "Tutti i prodotti sono stati eliminati." });
        }
    }
}
