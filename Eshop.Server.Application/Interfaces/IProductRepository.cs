using System.Collections.Generic;
using System.Threading.Tasks;
using Eshop.Server.Domain.Entities;

namespace Eshop.Server.Application.Interfaces
{
    /// Contratto di accesso/persistenza per l'aggregato Product.
    /// Lo strato Application dipende da questa interfaccia,
    /// mentre l'implementazione concreta vive nello strato Infrastruttura
    /// (es. tramite Entity Framework Core).
    public interface IProductRepository
    {
        /// Restituisce un product dato il suo ID, oppure null se non trovato.
        Task<Product?> GetByIdAsync(int id);

        /// Restituisce tutti i prodotti (es. per mostrare il catalogo).
        Task<IEnumerable<Product>> GetAllAsync();

        /// Salva un new product nel catalogo.
        Task AddAsync(Product product);

        /// Aggiorna i dati di un product existing.
        Task UpdateAsync(Product product);

        /// Rimuove un product dal catalogo.
        Task DeleteAsync(int id);

        /// Rimuove tutti i prodotti dal catalogo (operazione amministrativa o di test).
        Task DeleteAllAsync();
    }
}
