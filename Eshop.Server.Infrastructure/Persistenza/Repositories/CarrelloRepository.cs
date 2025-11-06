using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Domain.Modelli;
using Eshop.Server.Infrastructure.Persistenza;

namespace Eshop.Server.Infrastructure.Persistenza.Repositories
{
    /// <summary>
    /// Implementazione EF Core del repository per il Carrello.
    /// </summary>
    public class CarrelloRepository(AppDbContext context) : ICarrelloRepository
    {
        private readonly AppDbContext _context = context;

        /// <summary>
        /// Restituisce il carrello di un utente (con voci e prodotti).
        /// </summary>
        public async Task<Carrello?> GetByClienteIdAsync(int clienteId)
        {
            return await _context.Carrelli
                .Include(c => c.Voci)
                    .ThenInclude(v => v.Prodotto)
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);
        }

        /// <summary>
        /// Crea e salva un nuovo carrello (uso: creazione utente).
        /// </summary>
        public async Task AddAsync(Carrello carrello)
        {
            await _context.Carrelli.AddAsync(carrello);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Aggiorna il carrello esistente (quantità, voci, timestamp).
        /// Nota: si assume che l’entità provenga dal contesto corrente
        /// o che le modifiche siano tracciate; in caso contrario usiamo Update.
        /// </summary>
        public async Task UpdateAsync(Carrello carrello)
        {
            // Se il carrello non è tracciato, Update forza il tracking delle modifiche
            _context.Carrelli.Update(carrello);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Svuota completamente il carrello (rimuove tutte le voci).
        /// </summary>
        public async Task SvuotaAsync(int clienteId)
        {
            var carrello = await _context.Carrelli
                .Include(c => c.Voci)
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);

            if (carrello is null)
                return; // niente da fare

            if (carrello.Voci.Count > 0)
            {
                _context.VociCarrello.RemoveRange(carrello.Voci);
                carrello.Voci.Clear();
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina il carrello dell’utente (operazione rara).
        /// </summary>
        public async Task DeleteAsync(int clienteId)
        {
            var carrello = await _context.Carrelli
                .FirstOrDefaultAsync(c => c.ClienteId == clienteId);

            if (carrello is null)
                return;

            _context.Carrelli.Remove(carrello);
            await _context.SaveChangesAsync();
        }
    }
}
