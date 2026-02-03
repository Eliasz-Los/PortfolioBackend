using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.hospital.Caching;
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
    private readonly IPatientSearchCache _patientSearchCache;

    public PatientManager(IBaseRepository<Patient> repository, IMapper mapper, IValidation<Patient> validation, IPatientRepository patientRepository, IPatientSearchCache patientSearchCache)
    {
        _repository = repository;
        _mapper = mapper;
        _validation = validation;
        _patientRepository = patientRepository;
        _patientSearchCache = patientSearchCache;
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

    public async Task<IEnumerable<PatientDto>> SearchByFullNameOrDoB(string? term, CancellationToken cancellationToken = default)
    {
        var cached = await _patientSearchCache.TryGet(term, cancellationToken);
        if (cached != null)
        {
            return cached;
        }
        var patients = await _patientRepository.SearchPatientsByFullNameOrDateOfBirth(term, cancellationToken);
        var mapped = _mapper.Map<IReadOnlyList<PatientDto>>(patients);

        if (!string.IsNullOrWhiteSpace(term))
        {
            await _patientSearchCache.Set(term, mapped, ttl: TimeSpan.FromMinutes(5), cancellationToken);
        }
        return mapped;
    }
}