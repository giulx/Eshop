using Eshop.Server.Domain.Modelli;

namespace Eshop.Server.Application.Interfacce
{
    public interface IJwtService
    {
        string GeneraToken(Utente utente);
    }
}

