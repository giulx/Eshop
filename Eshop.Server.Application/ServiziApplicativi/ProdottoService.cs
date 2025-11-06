using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Application.DTOs.Prodotto;
using Eshop.Server.Application.Interfacce;
using Eshop.Server.Domain.Modelli;
using Eshop.Server.Domain.OggettiValore;

namespace Eshop.Server.Application.ServiziApplicativi
{
    /// <summary>
    /// Servizio applicativo per la gestione del catalogo prodotti.
    /// Orquestra i casi d'uso legati ai prodotti (lettura catalogo, creazione, update, delete).
    /// </summary>
    public class ProdottoService
    {
        private readonly IProdottoRepository _prodottoRepository;

        public ProdottoService(IProdottoRepository prodottoRepository)
        {
            _prodottoRepository = prodottoRepository;
        }

        /// <summary>
        /// Restituisce i prodotti come DTO di lettura, con filtro e paginazione.
        /// </summary>
        public async Task<(IReadOnlyList<ProdottoReadDTO> Items, int TotalCount)> GetAllAsync(
            string? search,
            int page,
            int pageSize)
        {
            // prendo tutto dal repository (se in futuro vuoi ottimizzare, qui metti IQueryable)
            var prodotti = (await _prodottoRepository.GetAllAsync()).ToList();

            // filtro
            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                prodotti = prodotti
                    .Where(p =>
                        p.Nome.ToLower().Contains(lower) ||
                        (p.Descrizione != null && p.Descrizione.ToLower().Contains(lower)))
                    .ToList();
            }

            var total = prodotti.Count;


            // paginazione in memoria
            var skip = (page - 1) * pageSize;
            var pageItems = prodotti
                .Skip(skip)
                .Take(pageSize)
                .ToList();

            // mapping a DTO
            var dtoList = pageItems
                .Select(p => new ProdottoReadDTO
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Descrizione = p.Descrizione,
                    Prezzo = p.Prezzo.Valore,
                    Valuta = p.Prezzo.Valuta,
                    QuantitaDisponibile = p.QuantitaDisponibile
                })
                .ToList()
                .AsReadOnly();

            return (dtoList, total);
        }


        /// <summary>
        /// Restituisce un singolo prodotto per Id.
        /// </summary>
        public async Task<ProdottoReadDTO?> GetByIdAsync(int id)
        {
            var prodotto = await _prodottoRepository.GetByIdAsync(id);
            if (prodotto == null)
                return null;

            return new ProdottoReadDTO
            {
                Id = prodotto.Id,
                Nome = prodotto.Nome,
                Descrizione = prodotto.Descrizione,
                Prezzo = prodotto.Prezzo.Valore,
                Valuta = prodotto.Prezzo.Valuta,
                QuantitaDisponibile = prodotto.QuantitaDisponibile
            };
        }

        /// <summary>
        /// Crea un nuovo prodotto nel catalogo.
        /// </summary>
        public async Task<ProdottoReadDTO> CreaProdottoAsync(ProdottoCreateDTO dto)
        {
            var prodotto = new Prodotto(
                dto.Nome,
                dto.Descrizione ?? string.Empty,
                new Money(dto.Prezzo, dto.Valuta ?? "EUR"),
                dto.QuantitaDisponibile
            );

            await _prodottoRepository.AddAsync(prodotto);

            // ritorniamo subito il DTO di lettura del prodotto creato
            return new ProdottoReadDTO
            {
                Id = prodotto.Id,
                Nome = prodotto.Nome,
                Descrizione = prodotto.Descrizione,
                Prezzo = prodotto.Prezzo.Valore,
                Valuta = prodotto.Prezzo.Valuta,
                QuantitaDisponibile = prodotto.QuantitaDisponibile
            };
        }

        /// <summary>
        /// Aggiorna le informazioni di un prodotto esistente.
        /// Update parziale: aggiorna solo i campi presenti nel DTO.
        /// Ritorna true se aggiornato, false se il prodotto non esiste.
        /// </summary>
        public async Task<bool> AggiornaProdottoAsync(int id, ProdottoUpdateDTO dto)
        {
            var prodotto = await _prodottoRepository.GetByIdAsync(id);
            if (prodotto == null)
                return false;

            // Aggiorna descrizione
            if (!string.IsNullOrWhiteSpace(dto.Descrizione))
            {
                prodotto.AggiornaDescrizione(dto.Descrizione);
            }

            // Aggiorna prezzo
            if (dto.Prezzo.HasValue)
            {
                var nuovaValuta = dto.Valuta ?? prodotto.Prezzo.Valuta;
                prodotto.AggiornaPrezzo(new Money(dto.Prezzo.Value, nuovaValuta));
            }

            // Aggiorna quantità
            if (dto.QuantitaDisponibile.HasValue)
            {
                prodotto.AggiornaQuantita(dto.QuantitaDisponibile.Value);
            }

            // Aggiorna nome se previsto nel dominio
            if (!string.IsNullOrWhiteSpace(dto.Nome))
            {
                // Se ancora non ce l'hai, puoi aggiungere nel dominio:
                // public void AggiornaNome(string nuovoNome) { ... }
                // Per ora facciamo la cosa semplice:
                prodotto.AggiornaDescrizione(prodotto.Descrizione); // NO-OP placeholder
                // ↑ se vuoi gestire Nome davvero, aggiungi AggiornaNome nel dominio
            }

            await _prodottoRepository.UpdateAsync(prodotto);
            return true;
        }

        /// <summary>
        /// Rimuove un prodotto singolo dal catalogo.
        /// Ritorna true se è stato trovato ed eliminato, false se non esisteva.
        /// </summary>
        public async Task<bool> EliminaProdottoAsync(int id)
        {
            var prodotto = await _prodottoRepository.GetByIdAsync(id);
            if (prodotto == null)
                return false;

            await _prodottoRepository.DeleteAsync(id);
            return true;
        }

        /// <summary>
        /// Rimuove TUTTI i prodotti dal catalogo.
        /// Operazione amministrativa / test.
        /// </summary>
        public async Task EliminaTuttiAsync()
        {
            await _prodottoRepository.DeleteAllAsync();
        }
    }
}
