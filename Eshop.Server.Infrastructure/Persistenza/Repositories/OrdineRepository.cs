using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Domain.Modelli;

namespace Eshop.Server.Infrastructure.Persistenza.Repositories
{
    /// <summary>
    /// Repository EF Core per l'aggregato Ordine.
    /// Carica sempre anche le voci (snapshot); il cliente solo dove ha senso.
    /// </summary>
    public class OrdineRepository : IOrdineRepository
    {
        private readonly AppDbContext _context;

        public OrdineRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Restituisce un singolo ordine con le sue voci e il cliente.
        /// </summary>
        public async Task<Ordine?> GetByIdAsync(int id)
        {
            return await _context.Ordini
                .Include(o => o.Voci)
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        /// <summary>
        /// Restituisce tutti gli ordini di un determinato cliente.
        /// Usato per "i miei ordini".
        /// </summary>
        public async Task<IEnumerable<Ordine>> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Ordini
                .Include(o => o.Voci)
                .Where(o => o.ClienteId == clienteId)
                .OrderByDescending(o => o.DataCreazione)
                .ToListAsync();
        }

        /// <summary>
        /// Restituisce tutti gli ordini (uso admin).
        /// </summary>
        public async Task<IEnumerable<Ordine>> GetAllAsync()
        {
            return await _context.Ordini
                .Include(o => o.Voci)
                .Include(o => o.Cliente)
                .OrderByDescending(o => o.DataCreazione)
                .ToListAsync();
        }

        /// <summary>
        /// Salva un nuovo ordine.
        /// </summary>
        public async Task AddAsync(Ordine ordine)
        {
            await _context.Ordini.AddAsync(ordine);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Aggiorna un ordine esistente (es. cambio stato).
        /// </summary>
        public async Task UpdateAsync(Ordine ordine)
        {
            _context.Ordini.Update(ordine);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine is null) return;

            _context.Ordini.Remove(ordine);
            await _context.SaveChangesAsync();
        }

        public async Task AggiornaStatoAsync(int id, StatoOrdine nuovoStato)
        {
            var ordine = await _context.Ordini.FindAsync(id);
            if (ordine is null)
                return;

            switch (nuovoStato)
            {
                case StatoOrdine.InElaborazione:
                    ordine.ImpostaInElaborazione();
                    break;
                case StatoOrdine.Spedito:
                    ordine.ImpostaSpedito();
                    break;
                case StatoOrdine.Cancellato:
                    ordine.Annulla();
                    break;
                default:
                    // Pagato non lo “imposti” da fuori di solito
                    throw new InvalidOperationException("Stato non impostabile manualmente.");
            }

            _context.Ordini.Update(ordine);
            await _context.SaveChangesAsync();
        }

    }
}
