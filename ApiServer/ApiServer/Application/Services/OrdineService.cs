using ApiServer.Application.DTOs.Ordine;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Models;

namespace ApiServer.Application.Services
{
    public class OrdineService
    {
        private readonly IOrdineRepository _ordineRepository;
        private readonly IUtenteRepository _utenteRepository;
        private readonly IProdottoRepository _prodottoRepository;

        public OrdineService(
            IOrdineRepository ordineRepository,
            IUtenteRepository utenteRepository,
            IProdottoRepository prodottoRepository)
        {
            _ordineRepository = ordineRepository;
            _utenteRepository = utenteRepository;
            _prodottoRepository = prodottoRepository;
        }

        public async Task<OrdineReadDTO?> GetByIdAsync(int id)
        {
            var ordine = await _ordineRepository.GetByIdAsync(id);
            if (ordine == null) return null;

            return new OrdineReadDTO
            {
                Id = ordine.Id,
                UtenteId = ordine.UtenteId,
                DataOrdine = ordine.DataOrdine,
                Totale = ordine.CalcolaTotale(),
                Articoli = ordine.Articoli.Select(a => new VoceOrdineDTO
                {
                    ProdottoId = a.ProdottoId,
                    NomeProdotto = a.Prodotto.Nome,
                    Quantita = a.Quantita,
                    PrezzoUnitario = a.PrezzoUnitario
                }).ToList()
            };
        }

        public async Task<bool> CreaOrdineAsync(OrdineCreateDTO dto)
        {
            var utente = await _utenteRepository.GetByIdAsync(dto.UtenteId);
            if (utente == null) return false;

            var ordine = new Ordine(utente.Id);

            foreach (var item in dto.Articoli)
            {
                var prodotto = await _prodottoRepository.GetByIdAsync(item.ProdottoId);
                if (prodotto == null) return false;

                ordine.AggiungiVoce(new VoceOrdine(
                    prodotto.Id,
                    prodotto.Nome,
                    prodotto.Prezzo,
                    item.Quantita));
            }

            await _ordineRepository.AddAsync(ordine);
            return true;
        }

        public async Task<bool> AggiornaOrdineAsync(int ordineId, OrdineUpdateDTO dto)
        {
            var ordine = await _ordineRepository.GetByIdAsync(ordineId);
            if (ordine == null) return false;

            ordine.AggiornaArticoli(dto.Articoli.Select(a =>
                new VoceOrdine(a.ProdottoId, "", 0m, a.Quantita)).ToList()); // Prezzo e nome li recuperi da Prodotto se vuoi

            await _ordineRepository.UpdateAsync(ordine);
            return true;
        }
    }
}
