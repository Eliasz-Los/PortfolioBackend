using System.Diagnostics;
using Domain;
using Domain.hospital;
using Domain.pathfinder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DAL.EntityFramework;

public class PortfolioDbContext : DbContext
{
    public DbSet<Floorplan> Floorplans { get; set; }
    public DbSet<Point> Points { get; set; }

    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=54326;Database=portfolio_db;Username=user;Password=postgres");
        }
        optionsBuilder.LogTo(message => Debug.WriteLine(message), LogLevel.Information);
    }

    public PortfolioDbContext(DbContextOptions options) : base(options)
    {
        PortfolioDbInitializer.Initialize(this, true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Floorplan>()
            .HasMany(f => f.Points)
            .WithOne(p => p.Floorplan)
            .OnDelete(DeleteBehavior.Cascade);
        
        // hospital
        
        modelBuilder.Entity<Patient>()
            .HasMany(patient => patient.Appointments)
            .WithOne(appointment => appointment.Patient)
            .HasForeignKey("PatientId");
        modelBuilder.Entity<Doctor>()
            .HasMany(doctor => doctor.Appointments)
            .WithOne(appointment => appointment.Doctor)
            .HasForeignKey("DoctorId");
        modelBuilder.Entity<Appointment>()
            .HasKey(a => a.Id);
        
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Patient)
            .WithMany(p => p.Invoices)
            .HasForeignKey("PatientId");
    }
}