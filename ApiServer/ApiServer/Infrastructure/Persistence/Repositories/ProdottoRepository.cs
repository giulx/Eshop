using Microsoft.EntityFrameworkCore;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Models;
using ApiServer.Infrastructure.Persistence;

namespace ApiServer.Infrastructure.Persistence.Repositories
{
    public class ProdottoRepository : IProdottoRepository
    {
        private readonly AppDbContext _context;

        public ProdottoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Prodotto?> GetByIdAsync(int id)
        {
            return await _context.Prodotti.FindAsync(id);
        }

        public async Task<List<Prodotto>> GetAllAsync()
        {
            return await _context.Prodotti.ToListAsync();
        }

        public async Task AddAsync(Prodotto prodotto)
        {
            _context.Prodotti.Add(prodotto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Prodotto prodotto)
        {
            _context.Prodotti.Update(prodotto);
            await _context.SaveChangesAsync();
        }
    }
}
