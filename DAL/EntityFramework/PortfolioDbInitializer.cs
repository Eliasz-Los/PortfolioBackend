using Domain;
using Domain.hospital;
using Domain.hospital.types;
using Domain.pathfinder;

namespace DAL.EntityFramework;

public class PortfolioDbInitializer
{
    public static void Initialize(PortfolioDbContext context, bool dropDatabase = false)
    {
        if (dropDatabase)
            context.Database.EnsureDeleted();
        if (context.Database.EnsureCreated())
            Seed(context);
    }

    private static void Seed(PortfolioDbContext context)
    {
        Floorplan floorplan1 = new Floorplan("teachers_floor", 1,"1/200", "teachers_floor1.png");
        Floorplan testFloorplan = new Floorplan("game_floor", 2,"1/200", "test_floorplan.png");
        Floorplan maze = new Floorplan("maze_floor", 3,"1/200", "maze.png");
        context.Floorplans.AddRange(floorplan1, testFloorplan, maze);
        
        //Hospital seed data
        var patients = GeneratePatients(100);
        context.Patients.AddRange(patients);

        var doctors = GenerateDoctors(10);
        context.Doctors.AddRange(doctors);

        var appointments = GenerateAppointments(50, patients, doctors);
        context.Appointments.AddRange(appointments);
        
        
        context.SaveChanges();
        
    }
    
    private static List<Patient> GeneratePatients(int count)
    {
        var patients = new List<Patient>();
        
        for (int i = 1; i <= count; i++)
        {
            var randomAge = new Random().Next(0, 60);
            patients.Add(new Patient(
                new Name($"PatientFirst{i}", $"PatientLast{i}"),
                DateOnly.FromDateTime(DateTime.Now.AddYears(-20 - randomAge)),
                $"patient{i}@mail.com",
                $"555-000{i:D3}",
                new Location($"Mortsel{i}", i, "Antwerp", "2640", "Belgium"),
                Guid.NewGuid()
                
            ));
        }
        return patients;
    }

    private static List<Doctor> GenerateDoctors(int count)
    {
        var doctors = new List<Doctor>();
        var specialisations = Enum.GetValues<Specialisation>();
        for (int i = 1; i <= count; i++)
        {
            var randomSpec = (Specialisation)specialisations.GetValue(new Random().Next(specialisations.Length))!;
            doctors.Add(new Doctor(
                new Name($"DoctorFirst{i}", $"DoctorLast{i}"),
                randomSpec,
                new Location("Antwerpsestraat", 10, "Antwerp", "2000", "Belgium"),
                Guid.NewGuid()
            ));
        }
        return doctors;
    }

    private static List<Appointment> GenerateAppointments(int count, List<Patient> patients, List<Doctor> doctors)
    {
        var appointments = new List<Appointment>();
        for (int i = 0; i < count; i++)
        {
            Doctor doc = doctors[i % doctors.Count];
            Patient patient = patients[i % doctors.Count];
            appointments.Add(new Appointment(
                DateTime.UtcNow.AddDays(i),
                patient,
                doc,
                Guid.NewGuid()));
        }
        return appointments;
    }
    
    
}