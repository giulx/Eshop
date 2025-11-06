using Microsoft.EntityFrameworkCore;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Models;
using ApiServer.Infrastructure.Persistence;

namespace ApiServer.Infrastructure.Persistence.Repositories
{
    public class UtenteRepository : IUtenteRepository
    {
        private readonly AppDbContext _context;

        public UtenteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Utente?> GetByIdAsync(int id)
        {
            return await _context.Utenti
                .Include(u => u.Ordini)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<Utente?> GetByEmailAsync(string email)
        {
            return await _context.Utenti
                .Include(u => u.Ordini)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddAsync(Utente utente)
        {
            _context.Utenti.Add(utente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Utente utente)
        {
            _context.Utenti.Update(utente);
            await _context.SaveChangesAsync();
        }
    }
}
