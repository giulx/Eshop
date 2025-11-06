namespace ApiServer.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(int userId, string email, string role);
        bool ValidateToken(string token, out int userId, out string email, out string role);
    }
}
