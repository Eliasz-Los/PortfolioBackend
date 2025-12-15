using DAL.EntityFramework;
using Domain.hospital;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.hospital;

public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(PortfolioDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<Appointment>> ReadAppointmentsByPatientId(Guid patientId)
    {
        return await _dbSet
            .Where(appointment => appointment.Patient.Id == patientId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Appointment>> ReadAppointmentsByDoctorId(Guid doctorId)
    {
        return await _dbSet
            .Where(appointment => appointment.Doctor.Id == doctorId)
            .ToListAsync();
    }
}