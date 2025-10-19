using AJT.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AJT.Services
{
    internal sealed class DbMigrationService : IHostedService
    {
        private readonly AJTDbContext _db;
        private readonly ILogger<DbMigrationService> _logger;

        public DbMigrationService(AJTDbContext db, ILogger<DbMigrationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning("Database is being migrated");
            await _db.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
