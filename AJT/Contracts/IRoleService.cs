namespace AJT.Contracts
{
    public interface IRoleService
    {
        Task<string> EncodeRoles(List<string> roles);
        Task<List<string>> DecodeRoles(string userRolesString);
        Task AddUserRole(Guid userId, string role);
        Task RemoveUserRole(Guid userId, string role);
    }
}
