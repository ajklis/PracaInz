using AJT.Contracts;
using AJT.DB;
using AJT.Options;
using AJT.Repositories;
using Microsoft.Extensions.DependencyInjection;

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

            services.Configure<AJTOptions>(opts => opts = options);

            return services;
        }
    }
}
