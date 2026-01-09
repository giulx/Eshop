using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Api.Extensions; // User.GetUserId()
using Eshop.Server.Application.DTOs.Order;
using Eshop.Server.Application.ApplicationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.Server.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrderController(OrderService orderService)
        {
            _orderService = orderService;
        }

        // ======================================================
        // CLIENTE: preview dal cart
        // ======================================================
        [HttpGet("preview")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(typeof(OrderPreviewDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPreview()
        {
            var userId = User.GetUserId()!.Value;

            var preview = await _orderService.PrepareOrderFromCartAsync(userId);
            if (preview is null)
                return NotFound(new { error = "Cart non trovato.", code = "cart_not_found" });

            return Ok(preview);
        }

        // ======================================================
        // CLIENTE: conferma order
        // ======================================================
        [HttpPost("conferma")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(typeof(ConfirmOrderResultDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Confirm()
        {
            var userId = User.GetUserId()!.Value;

            var result = await _orderService.ConfirmOrderFromCartAsync(userId);

            if (!result.Success)
            {
                return result.ErrorCode switch
                {
                    "stock_changed" => Conflict(new
                    {
                        error = "La disponibilità è cambiata. Rifai la preview.",
                        code = "stock_changed",
                        result.UnorderedRows
                    }),
                    "payment_failed" => BadRequest(new
                    {
                        error = "Pagamento non riuscito.",
                        code = "payment_failed"
                    }),
                    "no_items_orderable" => BadRequest(new
                    {
                        error = "Nessuna riga ordinabile nel cart.",
                        code = "no_items_orderable"
                    }),
                    "cart_not_found" => NotFound(new
                    {
                        error = "Carrello non trovato.",
                        code = "cart_not_found"
                    }),
                    _ => BadRequest(new
                    {
                        error = "Impossibile confermare l’order.",
                        code = result.ErrorCode ?? "unknown_error"
                    })
                };
            }

            return Ok(result);
        }

        // ======================================================
        // CLIENTE: i miei orders
        // ======================================================
        [HttpGet("miei")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(typeof(IEnumerable<OrderReadDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMiei()
        {
            var userId = User.GetUserId()!.Value;
            var orders = await _orderService.GetByCustomerAsync(userId);
            return Ok(orders);
        }

        // ======================================================
        // CLIENTE: dettaglio di un mio order
        // ======================================================
        [HttpGet("{id:int}")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(typeof(OrderReadDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMioOrder(int id)
        {
            var userId = User.GetUserId()!.Value;

            var order = await _orderService.GetByIdAsync(id);
            if (order is null || order.CustomerId != userId)
                return NotFound(new { error = "Order non trovato.", code = "order_not_found" });

            return Ok(order);
        }

        // ======================================================
        // CLIENTE: cancel un mio order
        // ======================================================
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "OnlyCustomer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelMioOrder(int id)
        {
            var userId = User.GetUserId()!.Value;

            var ok = await _orderService.CancelOrderByCustomerAsync(id, userId);
            if (!ok)
                return Conflict(new { error = "Order non tuo o non più cancelbile.", code = "cannot_cancel" });

            return Ok(new { message = "Order cancelto e rimborsato.", code = "cancelled" });
        }

        // ======================================================
        // ADMIN: tutti gli orders
        // ======================================================
        [HttpGet]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(typeof(IEnumerable<OrderReadDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _orderService.GetAllAsync();
            return Ok(orders);
        }

        // ======================================================
        // ADMIN: cancel order
        // ======================================================
        [HttpDelete("admin/{id:int}")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelByAdmin(int id)
        {
            var ok = await _orderService.CancelOrderByAdminAsync(id);
            if (!ok)
                return NotFound(new { error = "Order non trovato.", code = "order_not_found" });

            return Ok(new { message = "Order cancelto dall’amministratore.", code = "admin_cancelled" });
        }

        // ======================================================
        // ADMIN: eliminazione definitiva
        // ======================================================
        [HttpDelete("admin/{id:int}/hard")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> DeletePermanently(int id)
        {
            var ok = await _orderService.DeleteOrderPermanentlyAsync(id);
            if (!ok)
                return Conflict(new
                {
                    error = "Order non trovato o ancora nello status paid.",
                    code = "cannot_hard_delete"
                });

            return Ok(new { message = "Order eliminato definitivamente.", code = "deleted" });
        }

        // ======================================================
        // ADMIN: cambia status (nuovi)
        // ======================================================

        /// <summary>
        /// (Admin) Imposta l’order in status "In elaborazione".
        /// </summary>
        [HttpPut("admin/{id:int}/processing")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsProcessing(int id)
        {
            var ok = await _orderService.MarkAsProcessingByAdminAsync(id);
            if (!ok)
                return NotFound(new { error = "Order non trovato.", code = "order_not_found" });

            return Ok(new { message = "Order messo in elaborazione.", code = "set_in_elaborazione" });
        }

        /// <summary>
        /// (Admin) Imposta l’order in status "Shipped".
        /// </summary>
        [HttpPut("admin/{id:int}/shipped")]
        [Authorize(Policy = "OnlyAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsShipped(int id)
        {
            var ok = await _orderService.MarkAsShippedByAdminAsync(id);
            if (!ok)
                return NotFound(new { error = "Order non trovato.", code = "order_not_found" });

            return Ok(new { message = "Order segnato come shipped.", code = "set_shipped" });
        }
    }
}
