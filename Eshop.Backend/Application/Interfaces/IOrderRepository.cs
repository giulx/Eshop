using Eshop.Server.Domain.Entities;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshop.Server.Application.Interfaces
{
    /// <summary>
    /// Contratto per il repository dell'aggregato <see cref="Order"/>.
    /// Espone le operazioni di lettura e scrittura sugli orders,
    /// mantenendo l'Application Layer indipendente dall'implementazione (es. EF Core).
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Restituisce un order con le sue items e, opzionalmente, il customer associato.
        /// </summary>
        /// <param name="id">Identificativo univoco dell'order.</param>
        /// <returns>L'order se trovato, altrimenti <c>null</c>.</returns>
        Task<Order?> GetByIdAsync(int id);

        /// <summary>
        /// Restituisce tutti gli orders di un determinato customer.
        /// </summary>
        /// <param name="customerId">Identificativo del customer.</param>
        /// <returns>Enumerabile di orders appartenenti al customer.</returns>
        Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId);

        /// <summary>
        /// Restituisce tutti gli orders presenti nel sistema (solo per admin).
        /// </summary>
        /// <returns>Enumerabile di tutti gli orders.</returns>
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Aggiunge un new order al database.
        /// </summary>
        /// <param name="order">Order da salvare.</param>
        Task AddAsync(Order order);

        /// <summary>
        /// Aggiorna un order existing (es. cambio status, pagamento, ecc.).
        /// </summary>
        /// <param name="order">Order modificato da persistere.</param>
        Task UpdateAsync(Order order);

        /// <summary>
        /// Aggiorna solo lo status di un order (es. da Paid a Shipped).
        /// </summary>
        Task UpdateStatusAsync(int id, OrderStatus newStatus);


        Task DeleteAsync(int id);
    }
}
