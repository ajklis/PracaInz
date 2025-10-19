using AJT.Contracts;
using AJT.DB;
using AJT.Entities;
using Microsoft.EntityFrameworkCore;

namespace AJT.Repositories
{
    internal sealed class UserRoleRepo : IUserRoleRepo
    {
        private readonly AJTDbContext _db;

        public UserRoleRepo(AJTDbContext db)
        {
            _db = db;
        }

        public async Task AddUserRole(User user, Role role)
        {
            await _db.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
            await _db.SaveChangesAsync();
        }

        public async Task RemoveUserRole(UserRole userRole)
        {
            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync();
        }

        public async Task<UserRole?> GetUserRole(User user, Role role)
        {
            return await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == role.Id);
        }
    }
}
