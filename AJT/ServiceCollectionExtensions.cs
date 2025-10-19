using AJT.Contracts;
using AJT.DB;
using AJT.Options;
using AJT.Repositories;
using AJT.Services;
using Microsoft.Extensions.DependencyInjection;
using MOptions = Microsoft.Extensions.Options.Options;

namespace AJT
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds and configures ATJ
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection UseAJT(this IServiceCollection services, AJTOptions options, Func<IAJTConfigurator, IAJTConfigurator> configure)
        {
            // DB
            services.AddDbContext<AJTDbContext>();
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IRoleRepo, RoleRepo>();
            services.AddScoped<IUserRoleRepo, UserRoleRepo>();
            services.AddScoped<IRefreshTokenRepo, RefreshTokenRepo>();

            // bootstrapper, options, password hashing
            var configurator = (AJTConfigurator)configure(new AJTConfigurator());
            configurator.ApplyConfiguration(services, options);

            services.AddSingleton(MOptions.Create(options));

            // services
            services.AddSingleton<IHashingService, HashingService>();
            services.AddSingleton<ILoginService, LoginService>();
            services.AddSingleton<IRoleService, RoleService>();
            services.AddSingleton<ITokenDataService, TokenDataService>();

            return services;
        }
    }
}
