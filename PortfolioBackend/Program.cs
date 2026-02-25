using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using DAL.EntityFramework;
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
        var clientId = builder.Configuration["Keycloak:ClientId"] ?? "portfolio_client";


        options.Authority = authority;
        options.RequireHttpsMetadata = false; // local dev

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authority,
            ValidateAudience = true,
            ValidAudiences = new[] { "account", clientId, audience }.Where(x => !string.IsNullOrWhiteSpace(x)),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = ClaimTypes.NameIdentifier, 
            RoleClaimType = ClaimTypes.Role
        };
        
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Allow JWT in query string ONLY for SSE endpoint
                var path = context.HttpContext.Request.Path;

                if (path.StartsWithSegments("/api/docugroup/documents") &&
                    path.Value!.EndsWith("/events", StringComparison.OrdinalIgnoreCase))
                {
                    var token = context.Request.Query["access_token"].ToString();
                    if (!string.IsNullOrWhiteSpace(token))
                        context.Token = token;
                }

                return Task.CompletedTask;
            },
            
            OnTokenValidated = context =>
            {
                if (context.Principal?.Identity is not ClaimsIdentity identity)
                    return Task.CompletedTask;

                // Keycloak: resource_access.{clientId}.roles = [ "user", ... ]
                var resourceAccess = context.Principal.FindFirst("resource_access")?.Value;
                if (string.IsNullOrWhiteSpace(resourceAccess))
                    return Task.CompletedTask;

                using var doc = JsonDocument.Parse(resourceAccess);
                if (!doc.RootElement.TryGetProperty(clientId, out var clientNode))
                    return Task.CompletedTask;

                if (!clientNode.TryGetProperty("roles", out var rolesNode) || rolesNode.ValueKind != JsonValueKind.Array)
                    return Task.CompletedTask;

                foreach (var r in rolesNode.EnumerateArray())
                {
                    var role = r.GetString();
                    if (!string.IsNullOrWhiteSpace(role))
                        identity.AddClaim(new Claim(ClaimTypes.Role, role));
                }

                return Task.CompletedTask;
            }
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
