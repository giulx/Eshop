using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Domain.Entities;
using Eshop.Server.Domain.ValueObjects;

namespace Eshop.Server.Application.Interfaces
{
    /// <summary>
    /// Contratto per l'accesso e la persistenza degli users.
    /// Lo strato Application dipende da questa interfaccia;
    /// l'implementazione concreta vive nello strato Infrastruttura.
    /// </summary>
    public interface IUserRepository
    {
        //READ
        Task<User?> GetByIdAsync(int id);
        
        Task<IEnumerable<User>> GetAllAsync();

        //CREATE
        Task AddAsync(User user);

        //UPDATE
        Task UpdateAsync(User user);

        // DELETE
        Task<bool> DeleteAsync(int id);

        //CHECK EXISTENCE
        Task<User?> GetByEmailAsync(Email email);
        Task<bool> EmailExistsAsync(Email email);
    }
}

