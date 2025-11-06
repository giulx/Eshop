using ApiServer.Domain.Models;
using ApiServer.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Application.Interfaces
{
    public interface IUtenteRepository
    {
        Task<Utente?> GetByIdAsync(int id);
        Task<Utente?> GetByEmailAsync(Email email);
        Task<IEnumerable<Utente>> GetAllAsync();
        Task AddAsync(Utente utente);
        Task UpdateAsync(Utente utente);
        Task<bool> EmailEsistenteAsync(Email email);
    }
}

