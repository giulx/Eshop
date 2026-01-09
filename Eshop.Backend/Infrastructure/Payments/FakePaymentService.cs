using System;
using System.Globalization;
using System.Threading.Tasks;
using Eshop.Server.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Eshop.Server.Infrastructure.Payments
{
    /// <summary>
    /// Servizio di pagamento finto.
    /// Accetta i pagamenti fino a una certa soglia letta dal configuration,
    /// altrimenti li rifiuta. Utile per testare il ramo "pagamento fallito".
    /// </summary>
    public class FakePaymentService : IPaymentService
    {
        private readonly ILogger<FakePaymentService> _logger;
        private readonly decimal _maxImporto;

        public FakePaymentService(
            ILogger<FakePaymentService> logger,
            IConfiguration configuration)
        {
            _logger = logger;

            // leggiamo la chiave come stringa
            var raw = configuration["Pagamento:MaxImporto"];

            // fallback di default
            var valueDiDefault = 500m;

            if (!string.IsNullOrWhiteSpace(raw) &&
                decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
            {
                _maxImporto = parsed;
            }
            else
            {
                _maxImporto = valueDiDefault;
            }
        }

        public Task<bool> PayAsync(int customerId, decimal importo)
        {
            // regola finta: se l'importo è sopra la soglia, rifiuto
            if (importo > _maxImporto)
            {
                _logger.LogWarning(
                    "Pagamento rifiutato per customer {CustomerId}: importo {Importo} > soglia {Soglia}",
                    customerId, importo, _maxImporto);

                return Task.FromResult(false);
            }

            _logger.LogInformation(
                "Pagamento OK per customer {CustomerId}: importo {Importo}",
                customerId, importo);

            return Task.FromResult(true);
        }

        public Task<bool> RefundAsync(int customerId, decimal importo)
        {
            return Task.FromResult(true);
        }
    }
}
