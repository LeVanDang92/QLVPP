using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OSM.Application.Abstractions.Authentication;
using OSM.Application.Abstractions.Caching;
using OSM.Application.Abstractions.Data;
using OSM.Application.Abstractions.Excel;
using OSM.Application.Abstractions.Identity;
using OSM.Application.Abstractions.Messaging;
using OSM.Application.Abstractions.Storage;
using OSM.Application.Common.Emails;
using OSM.Infrastructure.Authentication;
using OSM.Infrastructure.Authorization;
using OSM.Infrastructure.Caching;
using OSM.Infrastructure.Data;
using OSM.Infrastructure.DomainEvents;
using OSM.Infrastructure.Emails;
using OSM.Infrastructure.Excel;
using OSM.Infrastructure.Identity;
using OSM.Infrastructure.Persistence;
using OSM.Infrastructure.Persistence.Interceptors;
using OSM.Infrastructure.Storage.Synology;
using System.Text;
using System.Text.Json;

namespace OSM.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddMemoryCache();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<AuditSaveChangesInterceptor>();
            services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddScoped<IDomainEventDispatcher, MediatRDomainEventDispatcher>();

            services
            .AddOptions<EmailConfiguration>()
            .Bind(configuration.GetSection(EmailConfiguration.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.From), "Email From is required.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.SmtpServer), "SMTP server is required.")
            .Validate(options => options.Port > 0, "SMTP port must be greater than 0.")
            .ValidateOnStart();

            services.AddTransient<IEmailSender, EmailSender>();
            services.AddScoped<IExcelReaderService, ExcelDataReaderService>();

            services.AddOptions<SynologyOptions>()
               .Bind(configuration.GetSection(SynologyOptions.SectionName))
               .ValidateDataAnnotations()
               .Validate(options => Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _), "Synology BaseUrl is invalid.")
               .ValidateOnStart();

            services.AddHttpClient<IFileStorageService, SynologyFileStorageService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<SynologyOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl.TrimEnd('/') + "/");
                client.Timeout = TimeSpan.FromMinutes(5);
            });

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

            services.AddOptions<JwtOptions>()
                 .Bind(configuration.GetSection(JwtOptions.SectionName))
                 .ValidateDataAnnotations()
                 .Validate(options => options.SecretKey.Length >= 32, "JWT secret key must be at least 32 characters.")
                 .Validate(options => !options.SecretKey.Contains("CHANGE_THIS", StringComparison.OrdinalIgnoreCase), "JWT secret key must be changed before running the application.")
                 .ValidateOnStart();

            services.AddOptions<TokenHashingOptions>()
                .Bind(configuration.GetSection(TokenHashingOptions.SectionName))
                .ValidateDataAnnotations()
                .Validate(options => options.Pepper.Length >= 32, "Token pepper must be at least 32 characters.")
                .Validate(options => !options.Pepper.Contains("CHANGE_THIS", StringComparison.OrdinalIgnoreCase), "Token pepper must be changed before running the application.")
                .ValidateOnStart();

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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
                        ClockSkew = TimeSpan.Zero // token hết hạn đúng thời điểm, không bị cộng thêm thời gian mặc định.
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/problem+json";

                            return context.Response.WriteAsync(JsonSerializer.Serialize(new
                            {
                                type = "https://httpstatuses.com/401",
                                title = "Unauthorized",
                                status = StatusCodes.Status401Unauthorized,
                                detail = "Authentication token is missing, invalid, or expired.",
                                traceId = context.HttpContext.TraceIdentifier
                            }));
                        },

                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/problem+json";

                            return context.Response.WriteAsync(JsonSerializer.Serialize(new
                            {
                                type = "https://httpstatuses.com/403",
                                title = "Forbidden",
                                status = StatusCodes.Status403Forbidden,
                                detail = "You do not have permission to access this resource.",
                                traceId = context.HttpContext.TraceIdentifier
                            }));
                        }
                    };
                });

            // Phân quyền dựa trên quyền (permission-based authorization)
            services.AddAuthorization();
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }
    }
}
