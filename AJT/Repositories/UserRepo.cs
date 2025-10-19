using AJT.Contracts;
using AJT.DB;
using AJT.Entities;
using Microsoft.EntityFrameworkCore;

namespace AJT.Repositories
{
    internal sealed class UserRepo : IUserRepo
    {
        private readonly AJTDbContext _db;

        public UserRepo(AJTDbContext db)
        {
            _db = db;
        }

        public async Task<User> AddUser(User user)
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateUser(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task RemoveUser(User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User?> GetUserById(Guid id)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetUserByLogin(string login)
        {
            return await _db.Users.FirstOrDefaultAsync(x => x.Username == login || x.Email == login);
        }
    }
}
