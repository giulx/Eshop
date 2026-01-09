using Microsoft.EntityFrameworkCore;
using Eshop.Server.Application.Interfaces;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;
using Eshop.Server.Infrastructure.Persistence;

namespace Eshop.Server.Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// Implementazione EF Core del repository per User.
    /// Gestisce l'accesso ai dati relativi agli users nel database.
    /// </summary>
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Orders)
                .Include(u => u.Cart!)
                    .ThenInclude(c => c.Items)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByEmailAsync(Email email)
        {
            // Normalizzazione difensiva: anche se Email VO normalizza già,
            // così evitiamo qualunque caso sporco (spazi/maiuscole).
            var needle = (email?.Value ?? string.Empty).Trim().ToLowerInvariant();
            if (needle.Length == 0) return null;

            return await _context.Users
                .Include(u => u.Orders)
                .Include(u => u.Cart)
                // confronto case-insensitive traducibile in SQL
                .FirstOrDefaultAsync(u => u.Email.Value.ToLower() == needle);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .Include(u => u.Orders)
                .Include(u => u.Cart)
                .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(Email email)
        {
            var needle = (email?.Value ?? string.Empty).Trim().ToLowerInvariant();
            if (needle.Length == 0) return false;

            return await _context.Users
                .AnyAsync(u => u.Email.Value.ToLower() == needle);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
