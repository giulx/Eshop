using Eshop.Server.Application.DTOs.Cart;
using Eshop.Server.Application.ApplicationServices;
using Eshop.Server.Api.Extensions; // per User.GetUserId()
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Eshop.Server.Api.Controllers
{
    [Authorize(Policy = "OnlyCustomer")]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly CartService _cartService;

        public CartController(CartService cartService)
        {
            _cartService = cartService;
        }

        // ==============================================
        // GET: api/cart/mio
        // ==============================================
        /// <summary>
        /// Ottiene il cart del customer loggato.
        /// Se il cart non esiste ancora, restituisce 200 OK con body null
        /// (che il frontend può interpretare come "carrello vuoto").
        /// </summary>
        [HttpGet("mio")]
        [SwaggerOperation(Summary = "Ottiene il cart del customer loggato")]
        [ProducesResponseType(typeof(CartReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetMioCart(CancellationToken ct)
        {
            var customerId = User.GetUserId();
            if (customerId is null)
                return Unauthorized();

            var cart = await _cartService.GetByCustomerIdAsync(customerId.Value);

            // 👇 Nessun carrello ancora presente → non consideriamo un errore
            if (cart is null)
            {
                // Il frontend potrà gestire "null" come "carrello vuoto"
                return Ok(null);
            }

            return Ok(cart);
        }

        // ==============================================
        // POST: api/cart/mio/item
        // ==============================================
        /// <summary>
        /// Aggiunge o incrementa una item nel cart del customer.
        /// </summary>
        [HttpPost("mio/item")]
        [SwaggerOperation(Summary = "Aggiunge o incrementa una item nel cart del customer")]
        [ProducesResponseType(typeof(CartReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = User.GetUserId();
            if (customerId is null)
                return Unauthorized();

            var ok = await _cartService.AddItemAsync(customerId.Value, dto.ProductId, dto.Quantity);
            if (!ok)
                return BadRequest(new { message = "Impossibile aggiungere il product al cart." });

            var cartUpdated = await _cartService.GetByCustomerIdAsync(customerId.Value);
            return Ok(cartUpdated);
        }

        // ==============================================
        // PUT: api/cart/mio/item
        // ==============================================
        /// <summary>
        /// Aggiorna la quantity di una item nel cart del customer.
        /// </summary>
        [HttpPut("mio/item")]
        [SwaggerOperation(Summary = "Aggiorna la quantity di una item nel cart del customer")]
        [ProducesResponseType(typeof(CartReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeQuantity([FromBody] ChangeCartQuantityDTO dto, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var customerId = User.GetUserId();
            if (customerId is null)
                return Unauthorized();

            var ok = await _cartService.ChangeQuantityAsync(customerId.Value, dto.ProductId, dto.Quantity);
            if (!ok)
                return NotFound(new { message = "Cart o item non trovati." });

            var cartUpdated = await _cartService.GetByCustomerIdAsync(customerId.Value);
            if (cartUpdated is null)
            {
                // Caso molto raro, ma meglio gestirlo
                return NotFound(new { message = "Cart non trovato dopo l’aggiornamento." });
            }

            return Ok(cartUpdated);
        }

        // ==============================================
        // DELETE: api/cart/mio/item
        // ==============================================
        /// <summary>
        /// Rimuove una item dal cart del customer.
        /// </summary>
        [HttpDelete("mio/item")]
        [SwaggerOperation(Summary = "Rimuove una item dal cart del customer")]
        [ProducesResponseType(typeof(CartReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveItem([FromBody] RemoveCartItemDTO dto, CancellationToken ct)
        {
            var customerId = User.GetUserId();
            if (customerId is null)
                return Unauthorized();

            var ok = await _cartService.RemoveItemAsync(customerId.Value, dto.ProductId);
            if (!ok)
                return NotFound(new { message = "Cart o item non trovati." });

            var cartUpdated = await _cartService.GetByCustomerIdAsync(customerId.Value);
            if (cartUpdated is null)
            {
                // Se rimuovendo l'ultima voce il cart viene "cancellato",
                // puoi decidere di restituire null per coerenza con il GET.
                return Ok(null);
            }

            return Ok(cartUpdated);
        }
    }
}
