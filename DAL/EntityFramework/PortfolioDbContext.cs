using System.Diagnostics;
using Domain;
using Domain.DocuGroup;
using Domain.hospital;
using Domain.hospital.types;
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
    
    public DbSet<GroupDocument> GroupDocuments { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<DocumentComponent> Components { get; set; }
    public DbSet<PublishEvent> PublishEvents { get; set; }
    
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
        PortfolioDbInitializer.Initialize(this, false);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Floorplan>()
            .HasMany(f => f.Points)
            .WithOne(p => p.Floorplan)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Hospital
        
        modelBuilder.Entity<Patient>()
            .HasMany(patient => patient.Appointments)
            .WithOne(appointment => appointment.Patient)
            .HasForeignKey("PatientId");
        modelBuilder.Entity<Patient>().OwnsOne(p => p.FullName, fn =>
        {
            fn.HasIndex("FirstName", "LastName"  )
                .HasDatabaseName("ix_patient_fullname");
        });
        modelBuilder.Entity<Patient>().OwnsOne(p => p.Location);
        modelBuilder.Entity<Patient>(p =>
        {
           
            p.HasIndex("DateOfBirth")
                .HasDatabaseName("ix_patient_dateofbirth");
        });
        
        modelBuilder.Entity<Doctor>()
            .HasMany(doctor => doctor.Appointments)
            .WithOne(appointment => appointment.Doctor)
            .HasForeignKey("DoctorId");
        modelBuilder.Entity<Doctor>().OwnsOne(d => d.FullName, fn =>
        {
            fn.HasIndex("FirstName", "LastName"  )
                .HasDatabaseName("ix_doctor_fullname");
        });
        modelBuilder.Entity<Doctor>().OwnsOne(d => d.WorkAddress);
        modelBuilder.Entity<Doctor>(d =>
        {
            d.HasIndex("Specialisation")
                .HasDatabaseName("ix_doctor_specialisation");
        });
        
        modelBuilder.Entity<Appointment>().HasKey(a => a.Id);
        
        modelBuilder.Entity<Invoice>()
            .HasOne(i => i.Patient)
            .WithMany(p => p.Invoices)
            .HasForeignKey("PatientId");

        // DocuGroup
        modelBuilder.Entity<GroupDocument>(doc =>
        {
            doc.HasMany(d => d.Components)
                .WithOne(c => c.GroupDocument)
                .HasForeignKey(c => c.GroupDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            doc.HasMany(d => d.Memberships)
                .WithOne(m => m.GroupDocument)
                .HasForeignKey(m => m.GroupDocumentId)
                .OnDelete(DeleteBehavior.Cascade);
            
            doc.HasMany( d=> d.Events)
                .WithOne(e => e.GroupDocument)
                .HasForeignKey(e => e.GroupDocumentId)
                .OnDelete(DeleteBehavior.Cascade);

        });

        modelBuilder.Entity<DocumentComponent>()
            .HasIndex(c => new { c.GroupDocumentId, c.Order })
            .IsUnique(false);

    }
}