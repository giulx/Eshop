using Eshop.Server.Domain.Entities;

namespace Eshop.Server.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}

