using BL.hospital;
using BL.hospital.Caching;
using BL.hospital.dto;
using BL.hospital.invoice;
using BL.hospital.mapper;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;

namespace PortfolioBackend.Services;

public static class HospitalDi
{
    public static IServiceCollection AddHospitalDi(this IServiceCollection services)
    { 
        // Repositories
       services.AddScoped<IAppointmentRepository, AppointmentRepository>();
       services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
       services.AddScoped<IPatientRepository, PatientRepository>();
       services.AddScoped<IDoctorRepository, DoctorRepository>();
        
        // Managers
       services.AddScoped<IBaseManager<Patient, PatientDto, AddPatientDto>, PatientManager> ();
       services.AddScoped<IPatientManager, PatientManager>();
       services.AddScoped<IBaseManager<Doctor, DoctorDto, AddDoctorDto>, DoctorManager> ();
       services.AddScoped<IDoctorManager, DoctorManager>();
       services.AddScoped<IBaseManager<Appointment, AppointmentDto, AddAppointmentDto>, AppointmentManager>();
       services.AddScoped<IAppointmentManager, AppointmentManager>();
       services.AddScoped<IInvoiceRepository, InvoiceRepository>();
       services.AddScoped<IInvoiceManager, InvoiceManager>();
       services.AddScoped<IValidation<Patient>, Validation<Patient>>();
       services.AddScoped<IValidation<Doctor>, Validation<Doctor>>();
       services.AddScoped<IValidation<Appointment>, Validation<Appointment>>();
       
       // Mappers
      services.AddAutoMapper(typeof(PatientMappingProfile));
      services.AddAutoMapper(typeof(AppointmentMappingProfile));
      services.AddAutoMapper(typeof(DoctorMappingProfile));
      services.AddAutoMapper(typeof(InvoiceMappingProfile));
      
      // Redis cache
      services.AddScoped<IDoctorSearchCache, DoctorSearchCache>();
      services.AddScoped<IPatientSearchCache, PatientSearchCache>();
        
        return services;
    }
}