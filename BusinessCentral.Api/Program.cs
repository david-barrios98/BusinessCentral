using BusinessCentral.Api.Extensions;
using BusinessCentral.Application.DTOs.Common;
using BusinessCentral.Infrastructure.DependencyInjection;
using BusinessCentral.Infrastructure.Persistence.Adapters;
using BusinessCentral.Infrastructure.Seed;
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
    .AddControllers();

builder.Services
    .AddCustomValidationResponse()
    .AddRoleExtensions();

// ================= LOGGING =================
builder.Logging.ClearProviders();
builder.Logging.AddNLog("nlog.config");

var app = builder.Build();

// ================= PIPELINE =================
app.UseGlobalMiddleware();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerDocumentation();
}

app.UseHttpsRedirection();
app.UseCors("_corsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ================= SEED =================
await app.RunSeedIfNeeded(configuration);

app.Run();