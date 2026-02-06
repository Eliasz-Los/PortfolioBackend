using System.Security.Claims;
using System.Text.Json.Serialization;
using BL.hospital;
using BL.hospital.Caching;
using BL.hospital.dto;
using BL.hospital.invoice;
using BL.hospital.mapper;
using BL.hospital.validation;
using BL.pathfinder;
using BL.Pathfinder;
using BL.Pathfinder.algorithm;
using BL.Pathfinder.analyzer;
using BL.pathfinder.mapper;
using DAL.EntityFramework;
using DAL.Repository;
using DAL.Repository.hospital;
using Domain.hospital;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PortfolioBackend.RateLimiting;
using PortfolioBackend.Services;
using QuestPDF.Infrastructure;

//License for QuestPDF for nowfree unless 1m revenue is reached
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PortfolioDbContext>( options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("portfolio_db")));

// Redis cache (optional in dev; requires Redis running)
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("redis");
    options.InstanceName = "PortfolioBackend:";
});


// Keycloak  + JWT
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var authority = builder.Configuration["Keycloak:Authority"];
        var audience = builder.Configuration["Keycloak:Audience"];

        options.Authority = authority;
        options.RequireHttpsMetadata = false; // local dev

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = ClaimTypes.NameIdentifier, // maps to `User.Identity.Name` if desired
            RoleClaimType = "realm_access.roles" // adjust if your roles are in a different claim
        };
    });

builder.Services.AddAuthorization();
//services
//Pathfinder
builder.Services.AddPathfinderDi();

//Hospital
builder.Services.AddHospitalDi();

// DocuGroup
builder.Services.AddDocuGroupDi();


builder.Services.AddOpenApi();
//So that the dotnet enum string support works
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });
builder.Services.AddAppRateLimiting();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.UseCors(policy => policy.WithOrigins("http://localhost:4200")
    .AllowAnyHeader()
    .AllowAnyMethod()
);

app.UseHttpsRedirection();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireRateLimiting(RateLimitingExtension.GlobalPolicy);

app.MapControllerRoute(name: "pathfinding", pattern:"api/path")
    .RequireRateLimiting(RateLimitingExtension.PathfindingPolicy);

app.Run();
