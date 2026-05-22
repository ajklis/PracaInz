using AJT.Entities;

namespace AJT.Contracts
{
    internal interface IRefreshTokenRepo
    {
        Task AddRefreshToken(RefreshToken refreshToken);
        Task<RefreshToken?> GetRefreshTokenForUserId(Guid userId);
        Task InvalidateRefreshToken(RefreshToken refreshToken);
        Task Remove(RefreshToken refreshToken);
    }
}
