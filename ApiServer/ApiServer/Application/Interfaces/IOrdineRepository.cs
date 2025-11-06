using ApiServer.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Application.Interfaces
{
    public interface IOrdineRepository
    {
        Task<Ordine?> GetByIdAsync(int id);
        Task<IEnumerable<Ordine>> GetByUtenteIdAsync(int utenteId);
        Task AddAsync(Ordine ordine);
        Task UpdateAsync(Ordine ordine);
    }
}
