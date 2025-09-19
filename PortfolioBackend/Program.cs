using System.Drawing;
using BL.mapper;
using BL.pathfinder;
using DAL.EntityFramework;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
    
builder.Services.AddDbContext<PortfolioDbContext>( options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("portfolio_db")));

// Add services to the container.
//services
builder.Services.AddScoped<PathManager>();
builder.Services.AddScoped<FloorplanRepository>();
builder.Services.AddScoped<FloorplanManager>();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
//mappers
builder.Services.AddAutoMapper(typeof(PointMappingProfile));

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

app.MapControllers();

app.Run();
