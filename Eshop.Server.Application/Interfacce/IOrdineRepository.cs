using Eshop.Server.Dominio.Modelli;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eshop.Server.Applicazione.Interfacce
{
    /// <summary>
    /// Contratto per il repository dell'aggregato <see cref="Ordine"/>.
    /// Espone le operazioni di lettura e scrittura sugli ordini,
    /// mantenendo l'Application Layer indipendente dall'implementazione (es. EF Core).
    /// </summary>
    public interface IOrdineRepository
    {
        /// <summary>
        /// Restituisce un ordine con le sue voci e, opzionalmente, il cliente associato.
        /// </summary>
        /// <param name="id">Identificativo univoco dell'ordine.</param>
        /// <returns>L'ordine se trovato, altrimenti <c>null</c>.</returns>
        Task<Ordine?> GetByIdAsync(int id);

        /// <summary>
        /// Restituisce tutti gli ordini di un determinato cliente.
        /// </summary>
        /// <param name="clienteId">Identificativo del cliente.</param>
        /// <returns>Enumerabile di ordini appartenenti al cliente.</returns>
        Task<IEnumerable<Ordine>> GetByClienteIdAsync(int clienteId);

        /// <summary>
        /// Restituisce tutti gli ordini presenti nel sistema (solo per admin).
        /// </summary>
        /// <returns>Enumerabile di tutti gli ordini.</returns>
        Task<IEnumerable<Ordine>> GetAllAsync();

        /// <summary>
        /// Aggiunge un nuovo ordine al database.
        /// </summary>
        /// <param name="ordine">Ordine da salvare.</param>
        Task AddAsync(Ordine ordine);

        /// <summary>
        /// Aggiorna un ordine esistente (es. cambio stato, pagamento, ecc.).
        /// </summary>
        /// <param name="ordine">Ordine modificato da persistere.</param>
        Task UpdateAsync(Ordine ordine);

        /// <summary>
        /// Aggiorna solo lo stato di un ordine (es. da Pagato a Spedito).
        /// </summary>
        Task AggiornaStatoAsync(int id, StatoOrdine nuovoStato);


        Task DeleteAsync(int id);
    }
}
