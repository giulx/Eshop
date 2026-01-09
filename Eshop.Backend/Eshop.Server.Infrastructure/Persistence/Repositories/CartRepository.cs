using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Infrastructure.Persistence;

namespace Eshop.Server.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementazione EF Core del repository per il Cart.
    /// </summary>
    public class CartRepository(AppDbContext context) : ICartRepository
    {
        private readonly AppDbContext _context = context;

        /// <summary>
        /// Restituisce il cart di un user (con items e prodotti).
        /// </summary>
        public async Task<Cart?> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }

        /// <summary>
        /// Crea e salva un new cart (uso: creazione user).
        /// </summary>
        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Aggiorna il cart existing (quantity, items, timestamp).
        /// Nota: si assume che l’entità provenga dal contesto corrente
        /// o che le modifiche siano tracciate; in caso contrario usiamo Update.
        /// </summary>
        public async Task UpdateAsync(Cart cart)
        {
            // Se il cart non è tracciato, Update forza il tracking delle modifiche
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Clear completamente il cart (rimuove tutte le items).
        /// </summary>
        public async Task ClearAsync(int customerId)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart is null)
                return; // niente da fare

            if (cart.Items.Count > 0)
            {
                _context.CartItems.RemoveRange(cart.Items);
                cart.Items.Clear();
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina il cart dell’user (operazione rara).
        /// </summary>
        public async Task DeleteAsync(int customerId)
        {
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (cart is null)
                return;

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
        }
    }
}
