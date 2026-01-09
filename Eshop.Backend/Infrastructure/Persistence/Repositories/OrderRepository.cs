using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;

namespace Eshop.Server.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Repository EF Core per l'aggregato Order.
    /// Carica sempre anche le items (snapshot); il customer solo dove ha senso.
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Restituisce un singolo order con le sue items e il customer.
        /// </summary>
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        /// <summary>
        /// Restituisce tutti gli orders di un determinato customer.
        /// Usato per "i miei orders".
        /// </summary>
        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(int customerId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.CreationDate)
                .ToListAsync();
        }

        /// <summary>
        /// Restituisce tutti gli orders (uso admin).
        /// </summary>
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .OrderByDescending(o => o.CreationDate)
                .ToListAsync();
        }

        /// <summary>
        /// Salva un new order.
        /// </summary>
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Aggiorna un order existing (es. cambio status).
        /// </summary>
        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is null) return;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order is null)
                return;

            switch (newStatus)
            {
                case OrderStatus.Processing:
                    order.MarkAsProcessing();
                    break;
                case OrderStatus.Shipped:
                    order.MarkAsShipped();
                    break;
                case OrderStatus.Cancelled:
                    order.Cancel();
                    break;
                default:
                    // Paid non lo “imposti” da fuori di solito
                    throw new InvalidOperationException("Status non impostabile manualmente.");
            }

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

    }
}
