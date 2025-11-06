using Eshop.Server.Dominio.Modelli;

namespace Eshop.Server.Applicazione.Interfacce
{
    public interface IJwtService
    {
        string GeneraToken(Utente utente);
    }
}

