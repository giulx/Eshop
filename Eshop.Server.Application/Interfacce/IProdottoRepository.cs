using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Dominio.Modelli;

namespace Eshop.Server.Applicazione.Interfacce
{
    /// Contratto di accesso/persistenza per l'aggregato Prodotto.
    /// Lo strato Applicazione dipende da questa interfaccia,
    /// mentre l'implementazione concreta vive nello strato Infrastruttura
    /// (es. tramite Entity Framework Core).
    public interface IProdottoRepository
    {
        /// Restituisce un prodotto dato il suo ID, oppure null se non trovato.
        Task<Prodotto?> GetByIdAsync(int id);

        /// Restituisce tutti i prodotti (es. per mostrare il catalogo).
        Task<IEnumerable<Prodotto>> GetAllAsync();

        /// Salva un nuovo prodotto nel catalogo.
        Task AddAsync(Prodotto prodotto);

        /// Aggiorna i dati di un prodotto esistente.
        Task UpdateAsync(Prodotto prodotto);

        /// Rimuove un prodotto dal catalogo.
        Task DeleteAsync(int id);

        /// Rimuove tutti i prodotti dal catalogo (operazione amministrativa o di test).
        Task DeleteAllAsync();
    }
}
