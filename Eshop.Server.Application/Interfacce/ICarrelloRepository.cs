using Eshop.Server.Dominio.Modelli;
using System.Threading.Tasks;

namespace Eshop.Server.Applicazione.Interfacce
{
    /// <summary>
    /// Contratto di persistenza per il Carrello.
    /// Ogni utente ha al massimo un carrello,
    /// creato automaticamente alla registrazione.
    /// </summary>
    public interface ICarrelloRepository
    {
        /// <summary>
        /// Restituisce il carrello dell’utente (sempre uno solo).
        /// </summary>
        Task<Carrello?> GetByClienteIdAsync(int clienteId);

        /// <summary>
        /// Crea e salva un nuovo carrello (usato solo alla creazione dell’utente).
        /// </summary>
        Task AddAsync(Carrello carrello);

        /// <summary>
        /// Aggiorna lo stato o le voci del carrello esistente.
        /// </summary>
        Task UpdateAsync(Carrello carrello);

        /// <summary>
        /// Svuota completamente il carrello (es. dopo pagamento avvenuto).
        /// </summary>
        Task SvuotaAsync(int clienteId);

        /// <summary>
        /// Elimina il carrello (operazione eccezionale, usata raramente).
        /// </summary>
        Task DeleteAsync(int clienteId);
    }
}
