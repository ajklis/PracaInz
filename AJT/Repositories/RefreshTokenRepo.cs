using AJT.Contracts;
using AJT.DB;
using AJT.Entities;
using Microsoft.EntityFrameworkCore;

namespace AJT.Repositories
{
    internal sealed class RefreshTokenRepo : IRefreshTokenRepo
    {
        private readonly AJTDbContext _db;

        public RefreshTokenRepo(AJTDbContext db)
        {
            _db = db;
        }

        public async Task AddRefreshToken(RefreshToken refreshToken)
        {
            await _db.RefreshTokens.AddAsync(refreshToken);
            await _db.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenForUserId(Guid userId)
        {
            return await _db.RefreshTokens.FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task InvalidateRefreshToken(RefreshToken refreshToken)
        {
            refreshToken.ExpirationDate = DateTime.Now;
            _db.RefreshTokens.Update(refreshToken);
            await _db.SaveChangesAsync();
        }
    }
}
