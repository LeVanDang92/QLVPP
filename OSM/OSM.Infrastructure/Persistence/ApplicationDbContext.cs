using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OSM.Application.Abstractions.Data;
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
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply all configurations from the assembly containing ApplicationDbContext
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly); 
        }
    }
}
