using Eshop.Server.Application.DTOs.Cart;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Eshop.Server.Application.ApplicationServices
{
    public class CartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(
            ICartRepository cartRepository,
            IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public async Task<CartReadDTO?> GetByCustomerIdAsync(int customerId)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart is null) return null;

            var items = cart.Items.Select(v => new CartItemDTO(
                ProductId: v.ProductId,
                Name: v.Product.Name,
                Quantity: v.Quantity.Value,
                UnitPriceSnapshot: v.Product.Price,
                Subtotal: v.TotalSnapshot()
            )).ToList();

            return new CartReadDTO(
                Id: cart.Id,
                CustomerId: cart.CustomerId,
                Items: items,
                Total: cart.CalculateTotalSnapshot()
            );
        }


        /// <summary>
        /// Aggiunge (o incrementa) una item nel cart dell’user.
        /// </summary>
        public async Task<bool> AddItemAsync(int customerId, int productId, int quantity)
        {
            // 1. prendo/creo il cart
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);

            // 2. prendo il product
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                return false; // oppure lanci eccezione

            // 3. creo VO quantity
            var qta = new Quantity(quantity);

            // 4. logica di dominio: è il cart che sa come aggiungere la item
            cart.AddOrIncrease(product, qta);

            // 5. salvo
            await _cartRepository.UpdateAsync(cart);

            return true;
        }

        /// <summary>
        /// Cambia la quantity di una item già presente.
        /// </summary>
        public async Task<bool> ChangeQuantityAsync(int customerId, int productId, int newQuantity)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null)
                return false;

            var qta = new Quantity(newQuantity);

            cart.UpdateQuantity(productId, qta);

            await _cartRepository.UpdateAsync(cart);
            return true;
        }

        /// <summary>
        /// Rimuove una item dal cart.
        /// </summary>
        public async Task<bool> RemoveItemAsync(int customerId, int productId)
        {
            var cart = await _cartRepository.GetByCustomerIdAsync(customerId);
            if (cart == null)
                return false;

            cart.RemoveItem(productId);

            await _cartRepository.UpdateAsync(cart);
            return true;
        }

        /// <summary>
        /// Clear il cart (es. dopo order).
        /// </summary>
        public Task ClearAsync(int customerId)
        {
            return _cartRepository.ClearAsync(customerId);
        }
    }
}
