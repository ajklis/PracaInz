using AJT.Attributes;
using AJT.Contracts;
using AJT.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace AJT.Services
{
    internal class RoleBootstrapper : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOptions<AJTOptions> _options;

        public RoleBootstrapper(IServiceScopeFactory scopeFactory, IOptions<AJTOptions> options)
        {
            _scopeFactory = scopeFactory;
            _options = options;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepo>();

            var existingRoles = await roleRepo.GetAllRoles();
            var configRoles = new List<(int Id, string Code)>();

            var list = _options.Value.DetectRolesFromAssembly
                ? GetDefinedRoles()
                : _options.Value.Roles;

            if (list is null || list.Count == 0)
                return;
            
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
            using var scope = _scopeFactory.CreateScope();
            var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepo>();
            var userRoleRepo = scope.ServiceProvider.GetRequiredService<IUserRoleRepo>();
            
            var currnetRoles = await roleRepo.GetAllRoles();
            var userRoles = await userRoleRepo.GetAllUserRoles();
            var userRoleNames = userRoles.Select(x => new
            {
                x.UserId,
                currnetRoles.Find(role => role.Id == x.RoleId)?.RoleCode
            }).ToList();

            // clear DB
            await roleRepo.RemoveAllRoles();

            // populate DB roles
            foreach (var (roleNumber, roleCode) in roles)
                await roleRepo.AddRole(new Entities.Role() { RoleNumber = roleNumber, RoleCode = roleCode });

            // revert user roles in DB
            var updatedRoles = await roleRepo.GetAllRoles();

            foreach (var userRole in userRoleNames)
            {
                var role = updatedRoles.FirstOrDefault(x => x.RoleCode == userRole.RoleCode);
                if (role is null)
                    continue;
                
                await userRoleRepo.AddUserRole(new Entities.User() { Id = userRole.UserId }, role);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private List<string> GetDefinedRoles()
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Role z klas
            var classRoles = assembly.GetTypes()
                .SelectMany(t => t.GetCustomAttributes<AllowRoleAttribute>(true))
                .SelectMany(a => a.Roles);

            // Role z metod
            var methodRoles = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .SelectMany(m => m.GetCustomAttributes<AllowRoleAttribute>(true))
                .SelectMany(a => a.Roles);

            return classRoles
                .Concat(methodRoles)
                .Distinct()
                .ToList();
        }
    }
}
