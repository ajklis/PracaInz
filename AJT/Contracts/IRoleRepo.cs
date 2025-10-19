using AJT.Entities;

namespace AJT.Contracts
{
    internal interface IRoleRepo
    {
        Task AddRole(Role role);
        Task RemoveRole(Role role);
        Task RemoveAllRoles();
        Task<Role?> GetRoleByCode(string code);
        Task<List<Role>> GetAllRoles();
    }
}
