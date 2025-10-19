using AJT.Contracts;
using AJT.DB;
using AJT.Entities;
using Microsoft.EntityFrameworkCore;

namespace AJT.Repositories
{
    internal sealed class RoleRepo : IRoleRepo
    {
        private readonly AJTDbContext _db;

        public RoleRepo(AJTDbContext db)
        {
            _db = db;
        }

        public async Task AddRole(Role role)
        {
            await _db.Roles.AddAsync(role);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveRole(Role role)
        {
            _db.Roles.Remove(role);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveAllRoles()
        {
            _db.Roles.RemoveRange(_db.Roles);
            await _db.SaveChangesAsync();
        }

        public async Task<Role?> GetRoleByCode(string code)
        {
            return await _db.Roles.FirstOrDefaultAsync(x => x.RoleCode == code);
        }

        public async Task<List<Role>> GetAllRoles()
        {
            return await _db.Roles.ToListAsync();
        }
    }
}
