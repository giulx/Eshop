using Eshop.Server.Dominio.Modelli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Server.Applicazione.Interfacce
{
    public interface IVoceCarrelloRepository
    {
        Task<VoceCarrello?> GetByCarrelloAndProdottoAsync(int carrelloId, int prodottoId);
        Task<IEnumerable<VoceCarrello>> GetByCarrelloIdAsync(int carrelloId);
        Task<VoceCarrello> CreateAsync(VoceCarrello voceCarrello);
        Task<VoceCarrello?> UpdateAsync(VoceCarrello voceCarrello); // Aggiorna quantità
        Task<bool> DeleteAsync(int id); // Rimuove singolo prodotto
        Task<bool> SvuotaCarrelloAsync(int carrelloId); // ✅ Svuota tutto al checkout
    }
}
