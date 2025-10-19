using AJT.Contracts;
using AJT.Options;
using AJT.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AJT
{
    internal class AJTConfigurator : IAJTConfigurator
    {
        private bool _customPasswordHashing = false;
        private bool _detectRoles = false;

        List<Action<IServiceCollection>> actions = new();

        public IAJTConfigurator AutomaticallyDetectRoles()
        {
            _detectRoles = true;
            return this;
        }

        public IAJTConfigurator UsePasswordHashing<T>() where T : class, IPasswordHasher
        {
            _customPasswordHashing = true;
            actions.Add(services => services.AddSingleton<IPasswordHasher, T>());
            return this;
        }

        public IAJTConfigurator UseRoleBootstrapper()
        {
            actions.Add(services => services.AddHostedService<RoleBootstrapper>());
            return this;
        }

        public void ApplyConfiguration(IServiceCollection services, AJTOptions options)
        {
            if (!_customPasswordHashing)
                services.AddSingleton<IPasswordHasher, MockPasswordHasher>();

            if (_detectRoles)
                options.DetectRolesFromAssembly = true;

            foreach (var action in actions)
                action(services);
        }

        public IAJTConfigurator MigrateDatabase()
        {
            actions.Add(services => services.AddHostedService<DbMigrationService>());
            return this;
        }

        public IAJTConfigurator AddDataToToken(Func<Guid, IServiceProvider, Task<object>> func)
        {
            TokenDataService.InitFunc = func;
            return this;
        }
    }
}
