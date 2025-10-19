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

        public async Task RemoveUserRole(User user, string role)
        {
            var roleEntity = await _db.Roles.Where(x => x.RoleCode == role).FirstOrDefaultAsync();
            if (roleEntity is null)
                return;

            var userRole = await _db.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.Id && x.RoleId == roleEntity.Id);
            if (userRole is null)
                return;

            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync();
        }

        public async Task<List<Role>?> GetUserRoles(User user)
        {
            var roleIds = await _db.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.RoleId).ToListAsync();
            return await _db.Roles.Where(x => roleIds.Contains(x.Id)).ToListAsync();
        }

        public async Task<List<UserRole>> GetAllUserRoles()
        {
            return await _db.UserRoles.ToListAsync();
        }
    }
}
