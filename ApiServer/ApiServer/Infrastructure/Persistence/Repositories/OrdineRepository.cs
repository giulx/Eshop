using Microsoft.EntityFrameworkCore;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Models;
using ApiServer.Infrastructure.Persistence;

namespace ApiServer.Infrastructure.Persistence.Repositories
{
    public class OrdineRepository : IOrdineRepository
    {
        private readonly AppDbContext _context;

        public OrdineRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Ordine?> GetByIdAsync(int id)
        {
            return await _context.Ordini
                .Include(o => o.Articoli)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Ordine>> GetByUtenteIdAsync(int utenteId)
        {
            return await _context.Ordini
                .Include(o => o.Articoli)
                .Where(o => o.UtenteId == utenteId)
                .ToListAsync();
        }

        public async Task AddAsync(Ordine ordine)
        {
            _context.Ordini.Add(ordine);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Ordine ordine)
        {
            _context.Ordini.Update(ordine);
            await _context.SaveChangesAsync();
        }
    }
}
