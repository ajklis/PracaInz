using AJT.Entities;

namespace AJT.Contracts
{
    public interface IUserManager
    {
        Task<User> AddUser(string username, string email, string hashedPassword);
        Task RemoveUserById(Guid userId);
        Task UpdateUser(User user);
    }
}
