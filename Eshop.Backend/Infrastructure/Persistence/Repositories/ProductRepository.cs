using Microsoft.EntityFrameworkCore;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshop.Server.Infrastructure.Persistence.Repositories
{
    /// Implementazione EF Core del repository per i prodotti.
    /// Gestisce tutte le operazioni CRUD sul database.
    public class ProductRepository(AppDbContext context) : IProductRepository
    {
        private readonly AppDbContext _context = context;

        /// Restituisce un product tramite il suo ID.
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FindAsync(id);
        }

        /// Restituisce tutti i prodotti presenti nel database.
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products.ToListAsync();
        }

        /// Aggiunge un new product e salva le modifiche nel database.
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        /// Aggiorna un product existing.
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        /// Elimina un singolo product in base al suo ID.
        /// NOTA: per evitare errori FK (CartItems -> Products ON DELETE RESTRICT),
        /// eseguiamo un "soft delete": ritiro del prodotto (IsActive=false + quantity=0).
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product is not null)
            {
                product.Deactivate();
                await _context.SaveChangesAsync();
            }
        }

        /// Elimina tutti i prodotti dal database.
        /// NOTA: soft delete in blocco per evitare errori FK.
        public async Task DeleteAllAsync()
        {
            var products = await _context.Products.ToListAsync();
            foreach (var p in products)
            {
                p.Deactivate();
            }

            await _context.SaveChangesAsync();
        }
    }
}
