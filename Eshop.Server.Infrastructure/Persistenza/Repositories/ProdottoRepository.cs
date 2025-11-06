using Microsoft.EntityFrameworkCore;
using Eshop.Server.Applicazione.Interfacce;
using Eshop.Server.Dominio.Modelli;
using Eshop.Server.Infrastruttura.Persistenza;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshop.Server.Infrastruttura.Persistenza.Repositories
{
    /// Implementazione EF Core del repository per i prodotti.
    /// Gestisce tutte le operazioni CRUD sul database.
    public class ProdottoRepository(AppDbContext context) : IProdottoRepository
    {
        private readonly AppDbContext _context = context;

        /// Restituisce un prodotto tramite il suo ID.
        public async Task<Prodotto?> GetByIdAsync(int id)
        {
            return await _context.Prodotti.FindAsync(id);
        }

        /// Restituisce tutti i prodotti presenti nel database.
        public async Task<IEnumerable<Prodotto>> GetAllAsync()
        {
            return await _context.Prodotti.ToListAsync();
        }

        /// Aggiunge un nuovo prodotto e salva le modifiche nel database.
        public async Task AddAsync(Prodotto prodotto)
        {
            await _context.Prodotti.AddAsync(prodotto);
            await _context.SaveChangesAsync();
        }

        /// Aggiorna un prodotto esistente.
        public async Task UpdateAsync(Prodotto prodotto)
        {
            _context.Prodotti.Update(prodotto);
            await _context.SaveChangesAsync();
        }

        /// Elimina un singolo prodotto in base al suo ID.
        public async Task DeleteAsync(int id)
        {
            var prodotto = await _context.Prodotti.FindAsync(id);
            if (prodotto is not null)
            {
                _context.Prodotti.Remove(prodotto);
                await _context.SaveChangesAsync();
            }
        }

        /// Elimina tutti i prodotti dal database.
        public async Task DeleteAllAsync()
        {
            _context.Prodotti.RemoveRange(_context.Prodotti);
            await _context.SaveChangesAsync();
        }
    }
}

