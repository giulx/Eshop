using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Order;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Application.ApplicationServices
{
    /// <summary>
    /// Gestisce il flusso:
    /// - prendo il cart
    /// - costruisco una preview (cosa posso davvero comprare adesso)
    /// - confermo → scalo stock → pago → creo order
    /// con compensazione se qualcosa va storto.
    /// Gestisce anche cancellazione (customer/admin) ed eliminazione definitiva (admin).
    /// </summary>
    public class OrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly IPaymentService _pagamentoService;

        public OrderService(
            IOrderRepository orderRepository,
            ICartRepository cartRepository,
            IProductRepository productRepository,
            IPaymentService pagamentoService)
        {
            _orderRepository = orderRepository;
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _pagamentoService = pagamentoService;
        }

        // =========================================================
        // 1) PREVIEW
        // =========================================================

        public async Task<OrderPreviewDTO?> PrepareOrderFromCartAsync(int customerId)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart is null)
                return null;

            var preview = new OrderPreviewDTO();

            foreach (var item in cart.Items)
            {
                var product = item.Product ?? await _productRepository.GetByIdAsync(item.ProductId);
                var demand = item.Quantity.Value;

                if (product is null)
                {
                    preview.DiscardedRows.Add(new UnorderableRowDTO
                    {
                        ProductId = item.ProductId,
                        Name = "(sconosciuto)",
                        Reason = "Product non trovato"
                    });
                    continue;
                }

                if (!product.CheckAvailability(demand))
                {
                    preview.DiscardedRows.Add(new UnorderableRowDTO
                    {
                        ProductId = product.Id,
                        Name = product.Name,
                        Reason = $"Disponibili solo {product.AvailableQuantity}"
                    });
                    continue;
                }

                preview.ValidRows.Add(new OrderRowDTO
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    UnitPrice = product.Price.Value,
                    Quantity = demand
                });
            }

            preview.Total = preview.ValidRows.Sum(r => r.Subtotal);
            preview.Token = Guid.NewGuid().ToString("N");

            return preview;
        }

        // =========================================================
        // 2) CONFERMA con COMPENSAZIONE
        // =========================================================

        public async Task<ConfirmOrderResultDTO> ConfirmOrderFromCartAsync(int customerId /*, string? token = null */)
        {
            var result = new ConfirmOrderResultDTO();

            // 1. foto aggiornata
            var preview = await PrepareOrderFromCartAsync(customerId);
            if (preview is null)
            {
                result.Success = false;
                result.ErrorCode = "cart_not_found";
                return result;
            }

            if (preview.ValidRows.Count == 0)
            {
                result.Success = false;
                result.ErrorCode = "no_items_orderable";
                result.UnorderedRows = preview.DiscardedRows;
                return result;
            }

            var decreasedProducts = new List<(int productId, int decreasedQuantity)>();

            // 2. SCALO lo stock (con check di nuovo)
            foreach (var riga in preview.ValidRows)
            {
                var product = await _productRepository.GetByIdAsync(riga.ProductId);
                if (product is null || !product.CheckAvailability(riga.Quantity))
                {
                    await RestoreStockAsync(decreasedProducts);
                    result.Success = false;
                    result.ErrorCode = "stock_changed";
                    result.UnorderedRows = preview.DiscardedRows;
                    return result;
                }

                product.DecreaseQuantity(riga.Quantity);
                await _productRepository.UpdateAsync(product);

                decreasedProducts.Add((product.Id, riga.Quantity));
            }

            // 3. PAGAMENTO
            var totalToPay = preview.Total;
            var paid = await _pagamentoService.PayAsync(customerId, totalToPay);
            if (!paid)
            {
                await RestoreStockAsync(decreasedProducts);
                result.Success = false;
                result.ErrorCode = "payment_failed";
                return result;
            }

            // 4. CREO ORDINE (nasce paid)
            var order = new Order(customerId);
            foreach (var riga in preview.ValidRows)
            {
                order.AddItem(
                    productId: riga.ProductId,
                    productName: riga.Name,
                    unitPrice: new Money(riga.UnitPrice, "EUR"),
                    quantity: riga.Quantity
                );
            }

            await _orderRepository.AddAsync(order);

            // 5. PULISCO CARRELLO
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart is not null)
            {
                foreach (var riga in preview.ValidRows)
                {
                    cart.RemoveItem(riga.ProductId);
                }

                await _cartRepository.UpdateAsync(cart);
            }

            // 6. DTO
            var totale = order.CalculateTotal();

            result.Success = true;
            result.Order = new OrderReadDTO
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreationDate = order.CreationDate,
                Status = order.Status,
                Total = totale.Value,
                Items = order.Items.Select(v => new OrderItemDTO
                {
                    ProductId = v.ProductId,
                    Name = v.ProductName,
                    UnitPrice = v.UnitPrice.Value,
                    Quantity = v.Quantity,
                    Subtotal = v.Subtotal.Value
                }).ToList()
            };

            return result;
        }

        private async Task RestoreStockAsync(List<(int productId, int quantityScalata)> prodottiScalati)
        {
            foreach (var (productId, quantity) in prodottiScalati)
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product is null)
                    continue;

                product.UpdateQuantity(product.AvailableQuantity + quantity);
                await _productRepository.UpdateAsync(product);
            }
        }

        // =========================================================
        // 3) LETTURA ORDINI
        // =========================================================

        public async Task<OrderReadDTO?> GetByIdAsync(int id)
        {
            var order = await _orderRepository.GetByIdAsync(id);
            if (order is null)
                return null;

            var totale = order.CalculateTotal();

            return new OrderReadDTO
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreationDate = order.CreationDate,
                Status = order.Status,
                Total = totale.Value,
                Items = order.Items.Select(v => new OrderItemDTO
                {
                    ProductId = v.ProductId,
                    Name = v.ProductName,
                    UnitPrice = v.UnitPrice.Value,
                    Quantity = v.Quantity,
                    Subtotal = v.Subtotal.Value
                }).ToList()
            };
        }

        public async Task<List<OrderReadDTO>> GetByCustomerAsync(int customerId)
        {
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId);

            return orders.Select(o =>
            {
                var tot = o.CalculateTotal();
                return new OrderReadDTO
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CreationDate = o.CreationDate,
                    Status = o.Status,
                    Total = tot.Value,
                    Items = o.Items.Select(v => new OrderItemDTO
                    {
                        ProductId = v.ProductId,
                        Name = v.ProductName,
                        UnitPrice = v.UnitPrice.Value,
                        Quantity = v.Quantity,
                        Subtotal = v.Subtotal.Value
                    }).ToList()
                };
            }).ToList();
        }

        public async Task<List<OrderReadDTO>> GetAllAsync()
        {
            var orders = await _orderRepository.GetAllAsync();

            return orders.Select(o =>
            {
                var tot = o.CalculateTotal();
                return new OrderReadDTO
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CreationDate = o.CreationDate,
                    Status = o.Status,
                    Total = tot.Value,
                    Items = o.Items.Select(v => new OrderItemDTO
                    {
                        ProductId = v.ProductId,
                        Name = v.ProductName,
                        UnitPrice = v.UnitPrice.Value,
                        Quantity = v.Quantity,
                        Subtotal = v.Subtotal.Value
                    }).ToList()
                };
            }).ToList();
        }

        // =========================================================
        // 4) CANCELLAZIONE / ELIMINAZIONE
        // =========================================================

        /// <summary>
        /// Cancellazione ordine da parte del customer.
        /// Consentita solo se l'ordine è stato pagato.
        /// Esegue rimborso + ripristino delle quantità a magazzino.
        /// </summary>
        public async Task<bool> CancelOrderByCustomerAsync(int orderId, int customerId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                return false;

            if (order.CustomerId != customerId)
                return false;

            // ✅ Solo ordini in stato "Paid" possono essere annullati dal cliente
            if (order.Status != OrderStatus.Paid)
                return false;

            // Mi salvo le righe per ripristinare lo stock
            var prodottiDaRipristinare = order.Items
                .Select(i => (i.ProductId, i.Quantity))
                .ToList();

            try
            {
                order.Cancel();
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            var totale = order.CalculateTotal().Value;
            if (totale > 0)
            {
                await _pagamentoService.RefundAsync(customerId, totale);
            }

            // ✅ Ripristino le quantità a magazzino
            await RestoreStockAsync(prodottiDaRipristinare);

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        /// <summary>
        /// Cancellazione forzata da admin.
        /// Esegue rimborso + ripristino delle quantità a magazzino.
        /// </summary>
        public async Task<bool> CancelOrderByAdminAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                return false;

            // Mi salvo le righe per ripristinare lo stock
            var prodottiDaRipristinare = order.Items
                .Select(i => (i.ProductId, i.Quantity))
                .ToList();

            order.ForceCancel();

            var totale = order.CalculateTotal().Value;
            if (totale > 0)
            {
                await _pagamentoService.RefundAsync(order.CustomerId, totale);
            }

            // ✅ Ripristino le quantità a magazzino
            await RestoreStockAsync(prodottiDaRipristinare);

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> DeleteOrderPermanentlyAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                return false;

            if (order.Status == OrderStatus.Paid)
                return false;

            await _orderRepository.DeleteAsync(orderId);
            return true;
        }

        // =========================================================
        // 5) GESTIONE STATO DA ADMIN
        // =========================================================

        /// <summary>
        /// Imposta un order in status "In elaborazione" (forzato da admin).
        /// </summary>
        public async Task<bool> MarkAsProcessingByAdminAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                return false;

            // usa il metodo di dominio che forza lo status
            order.MarkAsProcessingByAdmin();

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        /// <summary>
        /// Imposta un order in status "Shipped" (forzato da admin).
        /// </summary>
        public async Task<bool> MarkAsShippedByAdminAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order is null)
                return false;

            order.MarkAsShippedByAdmin();

            await _orderRepository.UpdateAsync(order);
            return true;
        }
    }
}
