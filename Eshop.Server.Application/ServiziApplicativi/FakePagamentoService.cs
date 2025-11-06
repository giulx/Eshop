using System;
using System.Globalization;
using System.Threading.Tasks;
using Eshop.Server.Application.Interfacce;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eshop.Server.Infrastructure.Pagamenti
{
    /// <summary>
    /// Servizio di pagamento finto.
    /// Accetta i pagamenti fino a una certa soglia letta dal configuration,
    /// altrimenti li rifiuta. Utile per testare il ramo "pagamento fallito".
    /// </summary>
    public class FakePagamentoService : IPagamentoService
    {
        private readonly ILogger<FakePagamentoService> _logger;
        private readonly decimal _maxImporto;

        public FakePagamentoService(
            ILogger<FakePagamentoService> logger,
            IConfiguration configuration)
        {
            _logger = logger;

            // leggiamo la chiave come stringa
            var raw = configuration["Pagamento:MaxImporto"];

            // fallback di default
            var valoreDiDefault = 500m;

            if (!string.IsNullOrWhiteSpace(raw) &&
                decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                _maxImporto = parsed;
            }
            else
            {
                _maxImporto = valoreDiDefault;
            }
        }

        public Task<bool> PagaAsync(int clienteId, decimal importo)
        {
            // regola finta: se l'importo è sopra la soglia, rifiuto
            if (importo > _maxImporto)
            {
                _logger.LogWarning(
                    "Pagamento rifiutato per cliente {ClienteId}: importo {Importo} > soglia {Soglia}",
                    clienteId, importo, _maxImporto);

                return Task.FromResult(false);
            }

            _logger.LogInformation(
                "Pagamento OK per cliente {ClienteId}: importo {Importo}",
                clienteId, importo);

            return Task.FromResult(true);
        }

        public Task<bool> RimborsoAsync(int clienteId, decimal importo)
        {
            return Task.FromResult(true);
        }
    }
}
