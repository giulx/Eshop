using System.Threading.Tasks;

namespace ApiServer.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<bool> ProcessaPagamentoAsync(decimal importo, string metodo, string riferimentoOrdine);
    }
}
