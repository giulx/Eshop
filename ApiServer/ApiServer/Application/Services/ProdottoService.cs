using ApiServer.Application.DTOs.Prodotto;
using ApiServer.Application.Interfaces;
using ApiServer.Domain.Models;

namespace ApiServer.Application.Services
{
    public class ProdottoService
    {
        private readonly IProdottoRepository _prodottoRepository;

        public ProdottoService(IProdottoRepository prodottoRepository)
        {
            _prodottoRepository = prodottoRepository;
        }

        public async Task<List<ProdottoReadDTO>> GetAllAsync()
        {
            var prodotti = await _prodottoRepository.GetAllAsync();
            return prodotti.Select(p => new ProdottoReadDTO
            {
                Id = p.Id,
                Nome = p.Nome,
                Descrizione = p.Descrizione,
                Prezzo = p.Prezzo,
                QuantitaDisponibile = p.QuantitaDisponibile
            }).ToList();
        }

        public async Task<ProdottoReadDTO?> GetByIdAsync(int id)
        {
            var prodotto = await _prodottoRepository.GetByIdAsync(id);
            if (prodotto == null) return null;

            return new ProdottoReadDTO
            {
                Id = prodotto.Id,
                Nome = prodotto.Nome,
                Descrizione = prodotto.Descrizione,
                Prezzo = prodotto.Prezzo,
                QuantitaDisponibile = prodotto.QuantitaDisponibile
            };
        }

        public async Task CreaProdottoAsync(ProdottoCreateDTO dto)
        {
            var prodotto = new Prodotto(dto.Nome, dto.Descrizione, dto.Prezzo, dto.QuantitaDisponibile);
            await _prodottoRepository.AddAsync(prodotto);
        }

        public async Task AggiornaProdottoAsync(int id, ProdottoUpdateDTO dto)
        {
            var prodotto = await _prodottoRepository.GetByIdAsync(id);
            if (prodotto == null) return;

            prodotto.AggiornaDettagli(dto.Nome, dto.Descrizione, dto.Prezzo, dto.QuantitaDisponibile);
            await _prodottoRepository.UpdateAsync(prodotto);
        }
    }
}
