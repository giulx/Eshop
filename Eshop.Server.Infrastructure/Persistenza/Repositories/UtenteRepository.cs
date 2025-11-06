using Microsoft.EntityFrameworkCore;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Domain.Modelli;
using Eshop.Server.Domain.OggettiValore;
using Eshop.Server.Infrastructure.Persistenza;

namespace Eshop.Server.Infrastructure.Persistenza.Repositories
{
    /// <summary>
    /// Implementazione EF Core del repository per Utente.
    /// Gestisce l'accesso ai dati relativi agli utenti nel database.
    /// </summary>
    public class UtenteRepository(AppDbContext context) : IUtenteRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<Utente?> GetByIdAsync(int id)
        {
            return await _context.Utenti
                .Include(u => u.Ordini)
                .Include(u => u.Carrello!)    
                    .ThenInclude(c => c.Voci) 
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Utente?> GetByEmailAsync(Email email)
        {
            return await _context.Utenti
                .Include(u => u.Ordini)
                .Include(u => u.Carrello)
                .FirstOrDefaultAsync(u => u.Email.Valore == email.Valore);
        }

        public async Task<IEnumerable<Utente>> GetAllAsync()
        {
            return await _context.Utenti
                .Include(u => u.Ordini)
                .Include(u => u.Carrello)
                .ToListAsync();
        }

        public async Task<bool> EmailEsistenteAsync(Email email)
        {
            return await _context.Utenti
                .AnyAsync(u => u.Email.Valore == email.Valore);
        }

        public async Task AddAsync(Utente utente)
        {
            await _context.Utenti.AddAsync(utente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Utente utente)
        {
            _context.Utenti.Update(utente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var utente = await _context.Utenti.FindAsync(id);
            if (utente == null)
                return false;

            _context.Utenti.Remove(utente);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
