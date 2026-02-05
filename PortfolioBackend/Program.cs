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
using Microsoft.EntityFrameworkCore;
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


//services
//Pathfinder
// builder.Services.AddScoped<IPathManager, PathManager>();
builder.Services.AddScoped<IPathfinding, AStarPathfinding>();
builder.Services.AddScoped<IFloorplanAnalyzer, FloorplanAnalyzer>();
builder.Services.AddScoped<IFloorplanRepository,FloorplanRepository>();
builder.Services.AddScoped<IFloorplanManager,FloorplanManager>();
builder.Services.AddScoped<ISecondAnalyzer, SecondFpAnalyzer>();
builder.Services.AddScoped<ISecondAStar, AStarGridPathfinding>();
builder.Services.AddScoped<IPathManager, SecondPathManager>();

//Hospital
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
//Hospital - Redis
builder.Services.AddScoped<IDoctorSearchCache, DoctorSearchCache>();
builder.Services.AddScoped<IPatientSearchCache, PatientSearchCache>();

builder.Services.AddScoped<IBaseManager<Patient, PatientDto, AddPatientDto>, PatientManager> ();
builder.Services.AddScoped<IPatientManager, PatientManager>();
builder.Services.AddScoped<IBaseManager<Doctor, DoctorDto, AddDoctorDto>, DoctorManager> ();
builder.Services.AddScoped<IDoctorManager, DoctorManager>();
builder.Services.AddScoped<IBaseManager<Appointment, AppointmentDto, AddAppointmentDto>, AppointmentManager>();
builder.Services.AddScoped<IAppointmentManager, AppointmentManager>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IInvoiceManager, InvoiceManager>();
builder.Services.AddScoped<IValidation<Patient>, Validation<Patient>>();
builder.Services.AddScoped<IValidation<Doctor>, Validation<Doctor>>();
builder.Services.AddScoped<IValidation<Appointment>, Validation<Appointment>>();

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

//mappers
builder.Services.AddAutoMapper(typeof(PointMappingProfile));
builder.Services.AddAutoMapper(typeof(PatientMappingProfile));
builder.Services.AddAutoMapper(typeof(AppointmentMappingProfile));
builder.Services.AddAutoMapper(typeof(DoctorMappingProfile));
builder.Services.AddAutoMapper(typeof(InvoiceMappingProfile));


var app = builder.Build();


//TODO later on 
//Applying EF Core migrations (creating tables /indexexs defined via OnModelCreating in DbContext)
/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
    //apply migrations
    dbContext.Database.Migrate();
}*/

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

app.MapControllers()
    .RequireRateLimiting(RateLimitingExtension.GlobalPolicy);

app.MapControllerRoute(name: "pathfinding", pattern:"api/path")
    .RequireRateLimiting(RateLimitingExtension.PathfindingPolicy);

app.Run();
