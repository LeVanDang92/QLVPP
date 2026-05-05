using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Data;
using OSM.Application.Abstractions.Identity;
using OSM.Infrastructure.Authentication;
using OSM.Infrastructure.Authorization;
using OSM.Infrastructure.Data;
using OSM.Infrastructure.Identity;
using OSM.Infrastructure.Persistence;
using OSM.Infrastructure.Persistence.Configurations;
using OSM.Infrastructure.Persistence.Interceptors;
using System.Text;

namespace OSM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<AuditSaveChangesInterceptor>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();

            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditSaveChangesInterceptor>();
                var connectionString = configuration.GetConnectionString("DefaultConnection")
                    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

                options.UseSqlServer(connectionString, sql =>
                {
                    sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
                options.AddInterceptors(interceptor);
            });

            services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                ?? throw new InvalidOperationException("Jwt configuration is missing.");

            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IIdentityService, IdentityService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.Read, p => p.Requirements.Add(new PermissionRequirement(PermissionCodes.Read)));
                options.AddPolicy(Policies.Write, p => p.Requirements.Add(new PermissionRequirement(PermissionCodes.Write)));
                options.AddPolicy(Policies.Delete, p => p.Requirements.Add(new PermissionRequirement(PermissionCodes.Delete)));
            });
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}
