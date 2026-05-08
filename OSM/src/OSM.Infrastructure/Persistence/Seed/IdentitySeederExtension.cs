using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSM.Infrastructure.Identity;

namespace OSM.Infrastructure.Persistence.Seed
{
    public static class IdentitySeederExtension
    {
        public static async Task SeedIdentity(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                var options = configuration.GetSection(IdentitySeedOptions.SectionName).Get<IdentitySeedOptions>()
                    ?? new IdentitySeedOptions();

                if (!options.Enabled)
                {
                    return;
                }

                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                await IdentitySeeder.SeedAsync(dbContext, userManager, roleManager, options);
            }
        }
    }
}
