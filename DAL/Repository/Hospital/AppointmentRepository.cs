using DAL.EntityFramework;
using Domain.hospital;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository.hospital;

public class AppointmentRepository : BaseRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(PortfolioDbContext context) : base(context)
    {
    }
    
    public async Task<Appointment?> ReadAppointmentWithRelationsById(Guid appointmentId)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .FirstOrDefaultAsync(a => a.Id == appointmentId);
    }
    
    public async Task<IEnumerable<Appointment>> ReadAppointmentsByPatientId(Guid patientId)
    {
        return await _dbSet
            .Include(a => a.Doctor)
            .Where(appointment => appointment.Patient.Id == patientId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Appointment>> ReadAppointmentsByDoctorId(Guid doctorId)
    {
        return await _dbSet
            .Include(a => a.Patient)
            .Where(appointment => appointment.Doctor.Id == doctorId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> ReadAppointmentsForDoctorInDateRange(Guid doctorId, DateTime from, DateTime to)
    {
        return await _dbSet.Include(a => a.Doctor)
            .Where(a => a.Doctor.Id == doctorId 
                && a.AppointmentDate >= from 
                && a.AppointmentDate <= to)
            .ToListAsync();
    }
}