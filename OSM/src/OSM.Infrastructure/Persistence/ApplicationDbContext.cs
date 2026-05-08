using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OSM.Application.Abstractions.Data;
using OSM.Domain.Common;
using OSM.Domain.Entities.Products;
using OSM.Infrastructure.Audit;
using OSM.Infrastructure.Identity;

namespace OSM.Infrastructure.Persistence
{
    public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options), IApplicationDbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RoleMenuPermission> RoleMenuPermissions => Set<RoleMenuPermission>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Menus> Menus => Set<Menus>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all configurations from the assembly containing ApplicationDbContext
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); 
        }

        /// <summary>
        /// DbContext sẽ quét tất cả entity đang được track ,gom event lại,publish
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
        {
            return ChangeTracker
                .Entries<IHasDomainEvents>()
                .SelectMany(entry => entry.Entity.DomainEvents)
                .ToList();
        }

        public void ClearDomainEvents()
        {
            var entities = ChangeTracker
                .Entries<IHasDomainEvents>()
                .Where(entry => entry.Entity.DomainEvents.Count != 0)
                .Select(entry => entry.Entity)
                .ToList();

            foreach (var entity in entities)
            {
                entity.ClearDomainEvents();
            }
        }

        public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
        {
            /// <summary>
            /// Khi chạy migration, EF Tool cần tạo DbContext trước khi app chạy
            /// </summary>
            /// <param name="args"></param>
            /// <returns></returns>
            public ApplicationDbContext CreateDbContext(string[] args)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json").Build();
                var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                builder.UseSqlServer(connectionString);
                return new ApplicationDbContext(builder.Options);
            }
        }
    }
}
