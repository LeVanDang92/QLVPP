using Microsoft.AspNetCore.Http.Features;
using OSM.API.Extensions;
using OSM.API.Middleware;
using OSM.API.Options;
using OSM.Application;
using OSM.Infrastructure;
using OSM.Infrastructure.Persistence.Seed;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Cấu hình FileUploadOptions và validate các giá trị cấu hình
builder.Services
    .AddOptions<FileUploadOptions>()
    .Bind(builder.Configuration.GetSection(FileUploadOptions.SectionName))
    .Validate(options => options.MaxFileSizeMb > 0, "MaxFileSizeMb must be greater than 0.")
    .Validate(options => options.AllowedExtensions.Length > 0, "AllowedExtensions must not be empty.")
    .ValidateOnStart();

var fileUploadOptions = builder.Configuration
    .GetSection(FileUploadOptions.SectionName)
    .Get<FileUploadOptions>() ?? new FileUploadOptions();

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = fileUploadOptions.MaxFileSizeBytes;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true; // If the client doesn't specify an API version, use the default version (1.0 in this case).
    options.ReportApiVersions = true;
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // Format version as "v1", "v2", etc.
    options.SubstituteApiVersionInUrl = true; // Tự thay {version} trong route thành version thật.
});

builder.Services.AddAppSwagger();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAppHealthChecks(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging(); // Log thông tin về request và response, bao gồm thời gian xử lý, mã trạng thái, v.v.

app.UseAppHealthChecks();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseAppSwagger();

    // Seed initial data
    await app.SeedIdentity();
}
else
{
    // In production, the React files will be served from this directory
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // xác thực user là ai.
app.UseAuthorization(); // kiểm tra user có quyền làm gì.

app.MapControllers();

app.Run();
