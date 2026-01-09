using Eshop.Server.Domain.Entities;
using System.Threading.Tasks;

namespace Eshop.Server.Application.Interfaces
{
    /// <summary>
    /// Contratto di persistenza per il Cart.
    /// Ogni user ha al massimo un cart,
    /// creato automaticamente alla registrazione.
    /// </summary>
    public interface ICartRepository
    {
        /// <summary>
        /// Restituisce il cart dell’user (sempre uno solo).
        /// </summary>
        Task<Cart?> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Crea e salva un new cart (usato solo alla creazione dell’user).
        /// </summary>
        Task AddAsync(Cart cart);

        /// <summary>
        /// Aggiorna lo status o le items del cart existing.
        /// </summary>
        Task UpdateAsync(Cart cart);

        /// <summary>
        /// Clear completamente il cart (es. dopo pagamento avvenuto).
        /// </summary>
        Task ClearAsync(int customerId);

        /// <summary>
        /// Elimina il cart (operazione eccezionale, usata raramente).
        /// </summary>
        Task DeleteAsync(int customerId);
    }
}
