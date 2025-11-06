using Eshop.Server.Applicazione.DTOs.Carrello;
using Eshop.Server.Applicazione.Interfacce;
using Eshop.Server.Dominio.Modelli;
using Eshop.Server.Dominio.OggettiValore;
using System.Threading.Tasks;

namespace Eshop.Server.Applicazione.ServiziApplicativi
{
    public class CarrelloService
    {
        private readonly ICarrelloRepository _carrelloRepository;
        private readonly IProdottoRepository _prodottoRepository;

        public CarrelloService(
            ICarrelloRepository carrelloRepository,
            IProdottoRepository prodottoRepository)
        {
            _carrelloRepository = carrelloRepository;
            _prodottoRepository = prodottoRepository;
        }

        public async Task<CarrelloReadDTO?> GetByClienteIdAsync(int clienteId)
        {
            var carrello = await _carrelloRepository.GetByClienteIdAsync(clienteId);
            if (carrello is null) return null;

            var voci = carrello.Voci.Select(v => new VoceCarrelloDTO(
                ProdottoId: v.ProdottoId,
                Nome: v.Prodotto.Nome,
                Quantita: v.Quantita.Valore,
                PrezzoUnitarioSnapshot: v.Prodotto.Prezzo,
                Subtotale: v.TotaleSnapshot()
            )).ToList();

            return new CarrelloReadDTO(
                Id: carrello.Id,
                ClienteId: carrello.ClienteId,
                Voci: voci,
                Totale: carrello.CalcolaTotaleSnapshot()
            );
        }


        /// <summary>
        /// Aggiunge (o incrementa) una voce nel carrello dell’utente.
        /// </summary>
        public async Task<bool> AggiungiVoceAsync(int clienteId, int prodottoId, int quantita)
        {
            // 1. prendo/creo il carrello
            var carrello = await _carrelloRepository.GetByClienteIdAsync(clienteId);

            // 2. prendo il prodotto
            var prodotto = await _prodottoRepository.GetByIdAsync(prodottoId);
            if (prodotto == null)
                return false; // oppure lanci eccezione

            // 3. creo VO quantita
            var qta = new Quantita(quantita);

            // 4. logica di dominio: è il carrello che sa come aggiungere la voce
            carrello.AggiungiOIncrementa(prodotto, qta);

            // 5. salvo
            await _carrelloRepository.UpdateAsync(carrello);

            return true;
        }

        /// <summary>
        /// Cambia la quantità di una voce già presente.
        /// </summary>
        public async Task<bool> CambiaQuantitaAsync(int clienteId, int prodottoId, int nuovaQuantita)
        {
            var carrello = await _carrelloRepository.GetByClienteIdAsync(clienteId);
            if (carrello == null)
                return false;

            var qta = new Quantita(nuovaQuantita);

            carrello.AggiornaQuantita(prodottoId, qta);

            await _carrelloRepository.UpdateAsync(carrello);
            return true;
        }

        /// <summary>
        /// Rimuove una voce dal carrello.
        /// </summary>
        public async Task<bool> RimuoviVoceAsync(int clienteId, int prodottoId)
        {
            var carrello = await _carrelloRepository.GetByClienteIdAsync(clienteId);
            if (carrello == null)
                return false;

            carrello.RimuoviVoce(prodottoId);

            await _carrelloRepository.UpdateAsync(carrello);
            return true;
        }

        /// <summary>
        /// Svuota il carrello (es. dopo ordine).
        /// </summary>
        public Task SvuotaAsync(int clienteId)
        {
            return _carrelloRepository.SvuotaAsync(clienteId);
        }
    }
}
