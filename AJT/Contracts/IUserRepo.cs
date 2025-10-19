using AJT.Entities;

namespace AJT.Contracts
{
    internal interface IUserRepo
    {
        Task<User> AddUser(User user);
        Task<User> UpdateUser(User user);
        Task RemoveUser(User user);
        Task<User?> GetUserById(Guid id);
        Task<User?> GetUserByLogin(string login);
    }
}
