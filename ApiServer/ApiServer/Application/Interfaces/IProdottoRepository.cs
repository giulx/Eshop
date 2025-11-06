using ApiServer.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApiServer.Application.Interfaces
{
    public interface IProdottoRepository
    {
        Task<Prodotto?> GetByIdAsync(int id);
        Task<IEnumerable<Prodotto>> GetAllAsync();
        Task AddAsync(Prodotto prodotto);
        Task UpdateAsync(Prodotto prodotto);
        Task DeleteAsync(int id);
    }
}
