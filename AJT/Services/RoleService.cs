using AJT.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace AJT.Services
{
    internal sealed class RoleService : IRoleService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public RoleService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<List<string>> DecodeRoles(string userRolesString)
        {
            using var scope = _scopeFactory.CreateScope();
            var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepo>();
            var dbRoles = await roleRepo.GetAllRoles();
            int byteCount = userRolesString.Length / 2;
            byte[] bytes = new byte[byteCount];
            for (int i = 0; i < byteCount; i++)
                bytes[i] = Convert.ToByte(userRolesString.Substring(i * 2, 2), 16);

            var result = new List<string>();

            for (int byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                for (int bit = 0; bit < 8; bit++)
                {
                    int bitIndex = byteIndex * 8 + bit;
                    bool isSet = (bytes[byteIndex] & (1 << bit)) != 0;
                    if (isSet)
                    {
                        var role = dbRoles.FirstOrDefault(r => r.RoleNumber == bitIndex)?.RoleCode;
                        if (role != null)
                            result.Add(role);
                    }
                }
            }

            return result;
        }

        public async Task<string> EncodeRoles(List<string> roles)
        {
            using var scope = _scopeFactory.CreateScope();
            var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepo>();
            var dbRoles = await roleRepo.GetAllRoles();
            var userRoles = new Dictionary<int, string>();

            int maxBit = 0;
            foreach (var role in roles)
            {
                var roleNumber = dbRoles.FirstOrDefault(x => x.RoleCode == role)?.RoleNumber ?? 0;
                userRoles.Add(roleNumber, role);
                if (maxBit < roleNumber)
                    maxBit = roleNumber;
            }

            int byteCount = (maxBit / 8) + 1;
            byte[] bytes = new byte[byteCount];

            foreach (var kv in userRoles)
            {
                int bitIndex = kv.Key;
                int byteIndex = bitIndex / 8;
                int bitInByte = bitIndex % 8;
                bytes[byteIndex] |= (byte)(1 << bitInByte);
            }

            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public async Task RemoveUserRole(Guid userId, string role)
        {
            using var scope = _scopeFactory.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();
            var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepo>();
            var userRoleRepo = scope.ServiceProvider.GetRequiredService<IUserRoleRepo>();

            var user = await userRepo.GetUserById(userId);
            if (user is null)
                return;

            await userRoleRepo.RemoveUserRole(user, role);
        }

        public async Task AddUserRole(Guid userId, string role)
        {
                using var scope = _scopeFactory.CreateScope();
            var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepo>();
            var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepo>();
            var userRoleRepo = scope.ServiceProvider.GetRequiredService<IUserRoleRepo>();

            var user = await userRepo.GetUserById(userId);
            if (user is null)
                return;

            var roleEntity = await roleRepo.GetRoleByCode(role);
            if (roleEntity is null)
                return;

            await userRoleRepo.AddUserRole(user, roleEntity);
        }
    }
}
