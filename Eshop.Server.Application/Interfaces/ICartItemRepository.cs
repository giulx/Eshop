using Eshop.Server.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Application.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByCartAndProductAsync(int cartId, int productId);
        Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId);
        Task<CartItem> CreateAsync(CartItem itemCart);
        Task<CartItem?> UpdateAsync(CartItem itemCart); // Aggiorna quantity
        Task<bool> DeleteAsync(int id); // Rimuove singolo product
        Task<bool> ClearCartAsync(int cartId); // ✅ Clear tutto al checkout
    }
}
