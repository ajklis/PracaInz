using AJT.Contracts;
using AJT.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AJT.Services
{
    internal class RoleBootstrapper : IHostedService
    {
        private readonly IRoleRepo _roleRepo;
        private readonly IUserRoleRepo _userRoleRepo;        
        private readonly IOptions<AJTOptions> _options;

        public RoleBootstrapper(IRoleRepo roleRepo, IUserRoleRepo userRoleRepo, IOptions<AJTOptions> options)
        {
            _roleRepo = roleRepo;
            _userRoleRepo = userRoleRepo;
            _options = options;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var existingRoles = await _roleRepo.GetAllRoles();
            var configRoles = new List<(int Id, string Code)>();

            var list = _options.Value.DetectRolesFromAssembly
                ? GetDefinedRoles()
                : _options.Value.Roles;

            
            for (int i = 0; i < list.Count; i++)
                configRoles.Add(new(i, list[i]));

            foreach (var (number, code) in configRoles)
            {
                var role = existingRoles.FirstOrDefault(x => x.RoleCode == code);

                if (role is null || role.RoleNumber != number)
                {
                    await ChangeRolesAsync(configRoles, cancellationToken);
                    return;
                }
            }
        }

        private async Task ChangeRolesAsync(List<(int number, string Code)> roles, CancellationToken cancellationToken)
        {
            // get DB data backed up
            var currnetRoles = await _roleRepo.GetAllRoles();
            var userRoles = await _userRoleRepo.GetAllUserRoles();
            var userRoleNames = userRoles.Select(x => new
            {
                x.UserId,
                currnetRoles.Find(role => role.Id == x.RoleId)?.RoleCode
            }).ToList();

            // clear DB
            await _roleRepo.RemoveAllRoles();

            // populate DB roles
            foreach (var (roleNumber, roleCode) in roles)
                await _roleRepo.AddRole(new Entities.Role() { RoleNumber = roleNumber, RoleCode = roleCode });

            // revert user roles in DB
            var updatedRoles = await _roleRepo.GetAllRoles();

            foreach (var userRole in userRoleNames)
            {
                var role = updatedRoles.FirstOrDefault(x => x.RoleCode == userRole.RoleCode);
                if (role is null)
                    continue;
                
                await _userRoleRepo.AddUserRole(new Entities.User() { Id = userRole.UserId }, role);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private List<string> GetDefinedRoles()
        {
            var assembly = Assembly.GetExecutingAssembly();

            return assembly
                .GetTypes()
                .SelectMany(t => t.GetCustomAttributes<AllowRoleAttribute>(true)
                    .Select(a => a.RoleCode))
                .Concat(
                    assembly.GetTypes()
                        .SelectMany(t => t.GetMethods())
                        .SelectMany(m => m.GetCustomAttributes<AllowRoleAttribute>(true)
                            .Select(a => a.RoleCode))
                )
                .Distinct()
                .ToList();
        }
    }
}
