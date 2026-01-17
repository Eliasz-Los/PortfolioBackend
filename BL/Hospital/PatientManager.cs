using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.generalDto;
using BL.hospital.dto;
using BL.hospital.mapper;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

public class PatientManager : IBaseManager<Patient, PatientDto, AddPatientDto>, IPatientManager  
{
    private readonly IMapper _mapper;
    private readonly IValidation<Patient> _validation;
    private readonly IBaseRepository<Patient> _repository;
    private readonly IPatientRepository _patientRepository;

    public PatientManager(IBaseRepository<Patient> repository, IMapper mapper, IValidation<Patient> validation, IPatientRepository patientRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _validation = validation;
        _patientRepository = patientRepository;
    }

    public async Task<PatientDto?> GetById(Guid patientId)
    {
        var patient = await _repository.ReadById(patientId);
        if (patient == null)
        {
            throw new KeyNotFoundException($"Patient with ID {patientId} not found.");
        }
        return _mapper.Map<PatientDto>(patient);
    }
    
    public async Task<IEnumerable<PatientDto>> GetAll()
    {
        var patients = await _repository.ReadAll();
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }
    
    public async Task<Patient> Add(AddPatientDto addPatient)
    {
        Patient patient = _mapper.Map<Patient>(addPatient);
        var validationResults = _validation.Validate(patient).ToList();
        
        if (validationResults.Any())
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            throw new ValidationException(errors);
        }
        
        return await _repository.Create(patient);
    }

    public void Remove(Guid id)
    { 
        _repository.Delete(id);
    }

    public async Task<IEnumerable<PatientDto>> Search(string? term, CancellationToken cancellationToken = default)
    {
        var patients = await _patientRepository.SearchPatientsByFullNameOrDateOfBirth(term, cancellationToken);
        return _mapper.Map<IEnumerable<PatientDto>>(patients);
    }
}