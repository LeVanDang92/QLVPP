using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace OSM.API.Extensions
{
    public static class HealthCheckExtensions
    {
        public static IServiceCollection AddAppHealthChecks(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            var connectionString = configuration.GetConnectionString("DefaultConnection")
               ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

            services
                .AddHealthChecks()
                .AddSqlServer(
                    connectionString: connectionString,
                    name: "sql-server",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["db", "sql", "ready"]);

            return services;
        }

        public static WebApplication UseAppHealthChecks(this WebApplication app)
        {
            // Check tổng quát
            app.MapHealthChecks("/health");

            // App process còn sống không (liveness) và app đã sẵn sàng nhận traffic chưa (readiness)
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = healthCheck => healthCheck.Tags.Contains("ready"),
                ResponseWriter = WriteHealthCheckResponse
            });

            // App đã sẵn sàng nhận request chưa, ví dụ SQL còn sống
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });

            return app;
        }

        private static async Task WriteHealthCheckResponse(
            HttpContext context,
            HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration.TotalMilliseconds,
                entries = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    duration = entry.Value.Duration.TotalMilliseconds,
                    error = entry.Value.Exception?.Message
                })
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}
