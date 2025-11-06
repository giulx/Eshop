using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshop.Server.Applicazione.DTOs.Ordine;
using Eshop.Server.Applicazione.Interfacce;
using Eshop.Server.Dominio.Modelli;
using Eshop.Server.Dominio.OggettiValore;

namespace Eshop.Server.Applicazione.ServiziApplicativi
{
    /// <summary>
    /// Gestisce il flusso:
    /// - prendo il carrello
    /// - costruisco una preview (cosa posso davvero comprare adesso)
    /// - confermo → scalo stock → pago → creo ordine
    /// con compensazione se qualcosa va storto.
    /// Gestisce anche cancellazione (cliente/admin) ed eliminazione definitiva (admin).
    /// </summary>
    public class OrdineService
    {
        private readonly IOrdineRepository _ordineRepository;
        private readonly ICarrelloRepository _carrelloRepository;
        private readonly IProdottoRepository _prodottoRepository;
        private readonly IPagamentoService _pagamentoService;

        public OrdineService(
            IOrdineRepository ordineRepository,
            ICarrelloRepository carrelloRepository,
            IProdottoRepository prodottoRepository,
            IPagamentoService pagamentoService)
        {
            _ordineRepository = ordineRepository;
            _carrelloRepository = carrelloRepository;
            _prodottoRepository = prodottoRepository;
            _pagamentoService = pagamentoService;
        }

        // =========================================================
        // 1) PREVIEW
        // =========================================================

        public async Task<OrdinePreviewDTO?> PreparaOrdineDaCarrelloAsync(int clienteId)
        {
            var carrello = await _carrelloRepository.GetByClienteIdAsync(clienteId);
            if (carrello is null)
                return null;

            var preview = new OrdinePreviewDTO();

            foreach (var voce in carrello.Voci)
            {
                var prodotto = voce.Prodotto ?? await _prodottoRepository.GetByIdAsync(voce.ProdottoId);
                var richiesta = voce.Quantita.Valore;

                if (prodotto is null)
                {
                    preview.RigheScartate.Add(new RigaNonOrdinabileDTO
                    {
                        ProdottoId = voce.ProdottoId,
                        Nome = "(sconosciuto)",
                        Motivo = "Prodotto non trovato"
                    });
                    continue;
                }

                if (!prodotto.HaDisponibilita(richiesta))
                {
                    preview.RigheScartate.Add(new RigaNonOrdinabileDTO
                    {
                        ProdottoId = prodotto.Id,
                        Nome = prodotto.Nome,
                        Motivo = $"Disponibili solo {prodotto.QuantitaDisponibile}"
                    });
                    continue;
                }

                preview.RigheValide.Add(new RigaOrdineDTO
                {
                    ProdottoId = prodotto.Id,
                    Nome = prodotto.Nome,
                    PrezzoUnitario = prodotto.Prezzo.Valore,
                    Quantita = richiesta
                });
            }

            preview.Totale = preview.RigheValide.Sum(r => r.Subtotale);
            preview.Token = Guid.NewGuid().ToString("N");

            return preview;
        }

        // =========================================================
        // 2) CONFERMA con COMPENSAZIONE
        // =========================================================

        public async Task<ConfermaOrdineResultDTO> ConfermaOrdineDaCarrelloAsync(int clienteId /*, string? token = null */)
        {
            var result = new ConfermaOrdineResultDTO();

            // 1. foto aggiornata
            var preview = await PreparaOrdineDaCarrelloAsync(clienteId);
            if (preview is null)
            {
                result.Success = false;
                result.ErrorCode = "cart_not_found";
                return result;
            }

            if (preview.RigheValide.Count == 0)
            {
                result.Success = false;
                result.ErrorCode = "no_items_orderable";
                result.RigheNonOrdinate = preview.RigheScartate;
                return result;
            }

            var prodottiScalati = new List<(int prodottoId, int quantitaScalata)>();

            // 2. SCALO lo stock (con check di nuovo)
            foreach (var riga in preview.RigheValide)
            {
                var prodotto = await _prodottoRepository.GetByIdAsync(riga.ProdottoId);
                if (prodotto is null || !prodotto.HaDisponibilita(riga.Quantita))
                {
                    await RipristinaStockAsync(prodottiScalati);
                    result.Success = false;
                    result.ErrorCode = "stock_changed";
                    result.RigheNonOrdinate = preview.RigheScartate;
                    return result;
                }

                prodotto.ScalaQuantita(riga.Quantita);
                await _prodottoRepository.UpdateAsync(prodotto);

                prodottiScalati.Add((prodotto.Id, riga.Quantita));
            }

            // 3. PAGAMENTO
            var totaleDaPagare = preview.Totale;
            var pagato = await _pagamentoService.PagaAsync(clienteId, totaleDaPagare);
            if (!pagato)
            {
                await RipristinaStockAsync(prodottiScalati);
                result.Success = false;
                result.ErrorCode = "payment_failed";
                return result;
            }

            // 4. CREO ORDINE (nasce pagato)
            var ordine = new Ordine(clienteId);
            foreach (var riga in preview.RigheValide)
            {
                ordine.AggiungiVoce(
                    prodottoId: riga.ProdottoId,
                    nomeProdotto: riga.Nome,
                    prezzoUnitario: new Money(riga.PrezzoUnitario, "EUR"),
                    quantita: riga.Quantita
                );
            }

            await _ordineRepository.AddAsync(ordine);

            // 5. PULISCO CARRELLO
            var carrello = await _carrelloRepository.GetByClienteIdAsync(clienteId);
            if (carrello is not null)
            {
                foreach (var riga in preview.RigheValide)
                {
                    carrello.RimuoviVoce(riga.ProdottoId);
                }

                await _carrelloRepository.UpdateAsync(carrello);
            }

            // 6. DTO
            var totale = ordine.CalcolaTotale();

            result.Success = true;
            result.Ordine = new OrdineReadDTO
            {
                Id = ordine.Id,
                ClienteId = ordine.ClienteId,
                DataCreazione = ordine.DataCreazione,
                Stato = ordine.Stato,
                Totale = totale.Valore,
                Voci = ordine.Voci.Select(v => new VoceOrdineDTO
                {
                    ProdottoId = v.ProdottoId,
                    Nome = v.NomeProdotto,
                    PrezzoUnitario = v.PrezzoUnitario.Valore,
                    Quantita = v.Quantita,
                    Subtotale = v.Subtotale.Valore
                }).ToList()
            };

            return result;
        }

        private async Task RipristinaStockAsync(List<(int prodottoId, int quantitaScalata)> prodottiScalati)
        {
            foreach (var (prodottoId, quantita) in prodottiScalati)
            {
                var prodotto = await _prodottoRepository.GetByIdAsync(prodottoId);
                if (prodotto is null)
                    continue;

                prodotto.AggiornaQuantita(prodotto.QuantitaDisponibile + quantita);
                await _prodottoRepository.UpdateAsync(prodotto);
            }
        }

        // =========================================================
        // 3) LETTURA ORDINI
        // =========================================================

        public async Task<OrdineReadDTO?> GetByIdAsync(int id)
        {
            var ordine = await _ordineRepository.GetByIdAsync(id);
            if (ordine is null)
                return null;

            var totale = ordine.CalcolaTotale();

            return new OrdineReadDTO
            {
                Id = ordine.Id,
                ClienteId = ordine.ClienteId,
                DataCreazione = ordine.DataCreazione,
                Stato = ordine.Stato,
                Totale = totale.Valore,
                Voci = ordine.Voci.Select(v => new VoceOrdineDTO
                {
                    ProdottoId = v.ProdottoId,
                    Nome = v.NomeProdotto,
                    PrezzoUnitario = v.PrezzoUnitario.Valore,
                    Quantita = v.Quantita,
                    Subtotale = v.Subtotale.Valore
                }).ToList()
            };
        }

        public async Task<List<OrdineReadDTO>> GetByClienteAsync(int clienteId)
        {
            var ordini = await _ordineRepository.GetByClienteIdAsync(clienteId);

            return ordini.Select(o =>
            {
                var tot = o.CalcolaTotale();
                return new OrdineReadDTO
                {
                    Id = o.Id,
                    ClienteId = o.ClienteId,
                    DataCreazione = o.DataCreazione,
                    Stato = o.Stato,
                    Totale = tot.Valore,
                    Voci = o.Voci.Select(v => new VoceOrdineDTO
                    {
                        ProdottoId = v.ProdottoId,
                        Nome = v.NomeProdotto,
                        PrezzoUnitario = v.PrezzoUnitario.Valore,
                        Quantita = v.Quantita,
                        Subtotale = v.Subtotale.Valore
                    }).ToList()
                };
            }).ToList();
        }

        public async Task<List<OrdineReadDTO>> GetAllAsync()
        {
            var ordini = await _ordineRepository.GetAllAsync();

            return ordini.Select(o =>
            {
                var tot = o.CalcolaTotale();
                return new OrdineReadDTO
                {
                    Id = o.Id,
                    ClienteId = o.ClienteId,
                    DataCreazione = o.DataCreazione,
                    Stato = o.Stato,
                    Totale = tot.Valore,
                    Voci = o.Voci.Select(v => new VoceOrdineDTO
                    {
                        ProdottoId = v.ProdottoId,
                        Nome = v.NomeProdotto,
                        PrezzoUnitario = v.PrezzoUnitario.Valore,
                        Quantita = v.Quantita,
                        Subtotale = v.Subtotale.Valore
                    }).ToList()
                };
            }).ToList();
        }

        // =========================================================
        // 4) CANCELLAZIONE / ELIMINAZIONE
        // =========================================================

        public async Task<bool> AnnullaOrdineDaClienteAsync(int ordineId, int clienteId)
        {
            var ordine = await _ordineRepository.GetByIdAsync(ordineId);
            if (ordine is null)
                return false;

            if (ordine.ClienteId != clienteId)
                return false;

            try
            {
                ordine.Annulla();
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            var totale = ordine.CalcolaTotale().Valore;
            if (totale > 0)
            {
                await _pagamentoService.RimborsoAsync(clienteId, totale);
            }

            await _ordineRepository.UpdateAsync(ordine);
            return true;
        }

        public async Task<bool> AnnullaOrdineDaAdminAsync(int ordineId)
        {
            var ordine = await _ordineRepository.GetByIdAsync(ordineId);
            if (ordine is null)
                return false;

            ordine.ForceCancel();

            var totale = ordine.CalcolaTotale().Valore;
            if (totale > 0)
            {
                await _pagamentoService.RimborsoAsync(ordine.ClienteId, totale);
            }

            await _ordineRepository.UpdateAsync(ordine);
            return true;
        }

        public async Task<bool> EliminaOrdineDefinitivoAsync(int ordineId)
        {
            var ordine = await _ordineRepository.GetByIdAsync(ordineId);
            if (ordine is null)
                return false;

            if (ordine.Stato == StatoOrdine.Pagato)
                return false;

            await _ordineRepository.DeleteAsync(ordineId);
            return true;
        }

        // =========================================================
        // 5) GESTIONE STATO DA ADMIN
        // =========================================================

        /// <summary>
        /// Imposta un ordine in stato "In elaborazione" (forzato da admin).
        /// </summary>
        public async Task<bool> MettiInElaborazioneDaAdminAsync(int ordineId)
        {
            var ordine = await _ordineRepository.GetByIdAsync(ordineId);
            if (ordine is null)
                return false;

            // usa il metodo di dominio che forza lo stato
            ordine.ImpostaInElaborazioneDaAdmin();

            await _ordineRepository.UpdateAsync(ordine);
            return true;
        }

        /// <summary>
        /// Imposta un ordine in stato "Spedito" (forzato da admin).
        /// </summary>
        public async Task<bool> SegnaSpeditoDaAdminAsync(int ordineId)
        {
            var ordine = await _ordineRepository.GetByIdAsync(ordineId);
            if (ordine is null)
                return false;

            ordine.ImpostaSpeditoDaAdmin();

            await _ordineRepository.UpdateAsync(ordine);
            return true;
        }
    }
}
