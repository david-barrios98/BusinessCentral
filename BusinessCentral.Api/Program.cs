using AspNetCoreRateLimit;
using BusinessCentral.Api.Extensions;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Infrastructure.DependencyInjection;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using BusinessCentral.Infrastructure.Seed;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using NLog.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using BusinessCentral.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

// ================= CONFIG =================
var configuration = builder.Configuration;

// ================= SERVICES =================
builder.Services
    .AddApplicationServices()
    .AddInfrastructure(configuration)
    .AddDatabase(configuration)
    .AddJwtAuthentication(configuration)
    .AddCorsPolicy(configuration)
    .AddRateLimiting(configuration)
    .AddSwaggerDocumentation()
    .AddTenantContext()
    .AddControllers();

builder.Services.AddHealthChecks();

builder.Services
    .AddCustomValidationResponse()
    .AddRoleExtensions();

// ================= LOGGING =================
builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");

var app = builder.Build();

// ================= PIPELINE =================
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseGlobalMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("_corsPolicy");

app.UseIpRateLimiting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// ================= SEED =================
await app.RunSeedIfNeeded(configuration);

app.Run();