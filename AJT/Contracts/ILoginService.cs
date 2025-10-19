using AJT.Models;

namespace AJT.Contracts
{
    public interface ILoginService
    {
        Task<CombinedToken?> Login(string login, string password);
        Task<bool> Register(string username, string email, string password);
        Task<CombinedToken?> Refresh(string refreshTokenString);

    }
}
