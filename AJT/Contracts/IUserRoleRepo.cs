using AJT.Entities;

namespace AJT.Contracts
{
    internal interface IUserRoleRepo
    {
        Task AddUserRole(User user, Role role);
        Task RemoveUserRole(UserRole userRole);
        Task<UserRole?> GetUserRole(User user, Role role);
        Task<List<UserRole>> GetAllUserRoles();
    }
}
