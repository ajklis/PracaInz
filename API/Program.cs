using AJT;
using AJT.Contracts;
using API.Services;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.UseAJT(
                new AJT.Options.AJTOptions
                {
                    ConnectionString = builder.Configuration.GetConnectionString("AJT"),
                    Secret = "super_sekretny_klucz",
                    TokenExpirationTime = TimeSpan.FromMinutes(10),
                    RefreshTokenExpirationTime = TimeSpan.FromDays(7),
                    Roles = GenerateRoles()
                },
                config => config.UseRoleBootstrapper()
                    //.UsePasswordHashing<PasswordHashingService>()
                    //.AutomaticallyDetectRoles()
                    .MigrateDatabase()
                    .AddDataToToken(AddInfo)
            );

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        static async Task<object> AddInfo(Guid userId, IServiceProvider services)
        {
            return "siema";
        }

        static List<string> GenerateRoles()
        {
            var roles = new List<string>
            {
                "admin"
            };
            for (int i = 0; i < 50; i++)
                roles.Add($"role{i}");
            return roles;
        }
    }
}
