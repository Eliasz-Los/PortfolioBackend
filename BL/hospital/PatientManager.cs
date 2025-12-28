using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.hospital.dto;
using BL.hospital.mapper;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

public class PatientManager : BaseManager<Patient>, IPatientManager
{
    private readonly IMapper _mapper;
    private readonly IValidation<Patient> _validation;

    public PatientManager(IBaseRepository<Patient> repository, IMapper mapper, IValidation<Patient> validation)
        : base(repository)
    {
        _mapper = mapper;
        _validation = validation;
    }

    public async Task<PatientDto> GetPatientById(Guid patientId)
    {
        var patient = await Repository.ReadById(patientId);
        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
        }
        return _mapper.Map<PatientDto>(patient);
    }
    
    public async Task<IEnumerable<PatientDto>> GetAllPatients()
    {
        var patients = await Repository.ReadAll();
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }
    
    public async Task<Patient> AddPatient(AddPatientDto addPatient)
    {
        Patient patient = _mapper.Map<Patient>(addPatient);
        var validationResults = _validation.Validate(patient).ToList();
        
        if (validationResults.Any())
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            throw new ValidationException(errors);
        }
        
        return await Repository.Create(patient);
    }

    public void RemovePatient(Guid id)
    { 
        Repository.Delete(id);
    }
   
}