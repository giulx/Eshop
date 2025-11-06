namespace Eshop.Server.Applicazione.Interfacce
{
    public interface IPagamentoService
    {
        Task<bool> PagaAsync(int clienteId, decimal importo);
        Task<bool> RimborsoAsync(int clienteId, decimal importo);
    }
}
