using AJT.Entities;
using AJT.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AJT.DB
{
    internal class AJTDbContext : DbContext
    {
        private readonly ILogger<AJTDbContext> _logger;
        private readonly string _connectionString;

        public AJTDbContext(IOptions<AJTOptions> options, ILogger<AJTDbContext> logger)
        {
            _logger = logger;
            _connectionString = options.Value.ConnectionString;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                if (!optionsBuilder.IsConfigured)
                    optionsBuilder.UseSqlServer(_connectionString);
            }
            catch (Exception e)
            {
                _logger.LogError("Database configuration error: {e}", e.Message);
                throw;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => ur.Id);

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne<Role>()
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ur => new { ur.UserId, ur.RoleId }).IsUnique();
            });
        }
    }
}
