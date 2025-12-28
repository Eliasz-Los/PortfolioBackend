using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

//TODO: we re back to having lots of managers, with the same functions,
// but because when calling the api it takes the include of appointments automatically
// I had to resort to specific managers for each entity, validation and mapping.
public class DoctorManager : BaseManager<Doctor>, IDoctorManager
{
    private readonly IMapper _mapper;
    private readonly IValidation<Doctor> _validation;
    
    public DoctorManager(IBaseRepository<Doctor> repository, IMapper mapper, IValidation<Doctor> validation) : base(repository)
    {
        _mapper = mapper;
        _validation = validation;
    }

    public async Task<DoctorDto> GetDoctorById(Guid id)
    {
        var doctor =  await Repository.ReadById(id);
        if (doctor == null)
        {
            throw new KeyNotFoundException($"Doctor with ID {id} not found.");
        }
        
        return _mapper.Map<DoctorDto>(doctor);
    }

    public async Task<IEnumerable<DoctorDto>> GetAllDoctors()
    {
        var doctors = await Repository.ReadAll();
        return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
    }

    public async Task<Doctor> AddDoctor(AddDoctorDto patient)
    {
        Doctor doctor = _mapper.Map<Doctor>(patient);
        var validationResults = _validation.Validate(doctor).ToList();
        if (validationResults.Any())
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            throw new ValidationException(errors);
        }
        
        return await Repository.Create(doctor);
        
    }

    public void RemoveDoctor(Guid id)
    {
        Repository.Delete(id);
    }
}