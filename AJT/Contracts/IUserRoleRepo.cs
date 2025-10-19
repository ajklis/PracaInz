using AJT.Entities;

namespace AJT.Contracts
{
    internal interface IUserRoleRepo
    {
        Task AddUserRole(User user, Role role);
        Task RemoveUserRole(User user, string role);
        Task<List<Role>?> GetUserRoles(User user);
        Task<List<UserRole>> GetAllUserRoles();
    }
}
