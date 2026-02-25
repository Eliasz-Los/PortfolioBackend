using Domain;
using Domain.DocuGroup;
using Domain.DocuGroup.types;
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
        var patients = GeneratePatients(10000);
        context.Patients.AddRange(patients);

        var doctors = GenerateDoctors(100);
        context.Doctors.AddRange(doctors);

        var appointments = GenerateAppointments(5000, patients, doctors);
        context.Appointments.AddRange(appointments);
        
        var invoices = GenerateInvoicesForPatients(patients);
        context.Invoices.AddRange(invoices);
        
        /*GroupDocument doc1 = new GroupDocument( 
            Guid.NewGuid(), 
            "Project Plan", 
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow);
        DocumentComponent comp1 = new DocumentComponent(Guid.NewGuid(), 1,"Introduction", doc1.Id, ComponentType.Title );
        DocumentComponent comp2 = new DocumentComponent(Guid.NewGuid(), 2,"woops", doc1.Id, ComponentType.Paragraph );
        
        
        doc1.Components.Add(comp1);
        doc1.Components.Add(comp2);
        context.GroupDocuments.Add(doc1);*/
        
        context.SaveChanges();
        
    }


    private static readonly string[] FirstNames =
    {
        "Adam", "Adrian", "Aiden", "Alan", "Albert", "Aleksander", "Alex", "Andrew", "Anthony", "Arthur",
        "Benjamin", "Bernard", "Blake", "Brandon", "Brian", "Caleb", "Cameron", "Carl", "Charles", "Christopher",
        "Daniel", "David", "Dominic", "Dylan", "Edward", "Elias", "Elliot", "Emil", "Eric", "Ethan",
        "Felix", "Finn", "Frank", "Gabriel", "George", "Gordon", "Harry", "Henry", "Hugo", "Ian",
        "Isaac", "Jack", "Jacob", "James", "Jason", "Jasper", "Jeremy", "Jonas", "Jonathan", "Jordan",
        "Joseph", "Julian", "Kai", "Kevin", "Kris", "Liam", "Logan", "Louis", "Lucas", "Luka",
        "Mark", "Martin", "Mateo", "Matthew", "Max", "Michael", "Milan", "Nathan", "Nicholas", "Noah",
        "Oliver", "Oscar", "Owen", "Patrick", "Paul", "Peter", "Philip", "Quentin", "Rafael", "Raymond",
        "Richard", "Robert", "Ryan", "Samuel", "Scott", "Sebastian", "Simon", "Stanley", "Stefan", "Thomas",
        "Timothy", "Tobias", "Tristan", "Victor", "Vincent", "Walter", "William", "Xavier", "Yannick", "Zachary"
    };
    
    private static readonly string[] LastNames =
    {
        "Adams","Anderson","Baker","Barnes","Bell","Bennett","Brooks","Brown","Campbell","Carter",
        "Clark","Coleman","Collins","Cook","Cooper","Cox","Davis","Diaz","Edwards","Evans",
        "Fisher","Fleming","Foster","Garcia","Gonzalez","Gray","Green","Hall","Harris","Hayes",
        "Henderson","Hughes","Jackson","James","Jenkins","Johnson","Jones","Kelly","King","Lee",
        "Lewis","Lopez","Martin","Martinez","Miller","Mitchell","Moore","Morgan","Morris","Murphy",
        "Nelson","Nguyen","Parker","Perez","Phillips","Powell","Price","Ramirez","Reed","Richardson",
        "Rivera","Roberts","Robinson","Rodriguez","Rogers","Ross","Russell","Sanders","Scott","Simmons",
        "Smith","Stewart","Taylor","Thomas","Thompson","Torres","Turner","Walker","Ward","Watson",
        "White","Williams","Wilson","Wood","Wright","Young","Nowak","Kowalski","Wojcik","Kaminski"
    };
    private static List<Patient> GeneratePatients(int count)
    {
        var patients = new List<Patient>();
        
        for (int i = 1; i <= count; i++)
        {
            var randomAge = new Random().Next(0, 60);
            var first = FirstNames[new Random().Next(FirstNames.Length)];
            var last = LastNames[new Random().Next(LastNames.Length)];
            var email = $"{first.ToLower()}.{last.ToLower()}.{i}@mail.com";
            var phone = $"+32 4{new Random().Next(10000000, 99999999)}";

            patients.Add(new Patient(
                new Name(first, last),
                DateOnly.FromDateTime(DateTime.Now.AddYears(-20 - randomAge)),
                email,
                phone,
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
            var first = FirstNames[new Random().Next(FirstNames.Length)];
            var last = LastNames[new Random().Next(LastNames.Length)];
            
            var randomSpec = (Specialisation)specialisations.GetValue(new Random().Next(specialisations.Length))!;
            doctors.Add(new Doctor(
                new Name(first, last),
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
        var rand = new Random();

        for (int i = 0; i < count; i++)
        {
            Doctor doc = doctors[rand.Next(doctors.Count)];
            Patient patient = patients[rand.Next(patients.Count)];
            appointments.Add(new Appointment(
                Guid.NewGuid(),
                patient.Id,
                doc.Id,
                DateTime.UtcNow.AddDays(i)));
        }
        return appointments;
    }
    
    private static List<Invoice> GenerateInvoicesForPatients(List<Patient> patients)
    {
        var invoices = new List<Invoice>();
        var rand = new Random();

        foreach (var patient in patients)
        {
            int invoiceCount = rand.Next(1, 3);

            for (int i = 1; i <= invoiceCount; i++)
            {
                var amount = rand.Next(50, 1000); 
                var invoiceDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-rand.Next(30)));
                var dueDate = invoiceDate.AddDays(30); 
                var invoiceNumber = $"INV-{patient.Id.ToString().Substring(0, 8).ToUpper()}-{i}";
                
                
                invoices.Add(new Invoice(
                    invoiceNumber,
                    invoiceDate,
                    amount,
                    "Hospital Invoice",
                    "Services rendered by the Hospital.",
                    dueDate,
                    Guid.NewGuid(),
                    isPaid: rand.NextDouble() > 0.5
                )
                {
                    Patient = patient,
                 
                });
            }
        }

        return invoices;
    }

    
    
}