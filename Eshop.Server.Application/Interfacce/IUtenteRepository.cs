using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Domain.Modelli;
using Eshop.Server.Domain.OggettiValore;

namespace Eshop.Server.Application.Interfacce
{
    /// <summary>
    /// Contratto per l'accesso e la persistenza degli utenti.
    /// Lo strato Application dipende da questa interfaccia;
    /// l'implementazione concreta vive nello strato Infrastruttura.
    /// </summary>
    public interface IUtenteRepository
    {
        //READ
        Task<Utente?> GetByIdAsync(int id);
        
        Task<IEnumerable<Utente>> GetAllAsync();

        //CREATE
        Task AddAsync(Utente utente);

        //UPDATE
        Task UpdateAsync(Utente utente);

        // DELETE
        Task<bool> DeleteAsync(int id);

        //CHECK EXISTENCE
        Task<Utente?> GetByEmailAsync(Email email);
        Task<bool> EmailEsistenteAsync(Email email);
    }
}

