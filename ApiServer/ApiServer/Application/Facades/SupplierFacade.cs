namespace ApiServer.Facades
{
    public class SupplierFacade
    {
        // Metodo minimale che simula il rifornimento di un prodotto
        public bool OrderFromSupplier(int prodottoId, int quantita)
        {
            // Qui normalmente chiameresti un servizio esterno
            // Per ora ritorniamo sempre true per simulare successo
            return true;
        }
    }
}

