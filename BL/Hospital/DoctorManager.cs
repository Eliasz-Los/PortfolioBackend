using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BL.hospital.Caching;
using BL.hospital.dto;
using BL.hospital.validation;
using DAL.Repository.hospital;
using Domain.hospital;

namespace BL.hospital;

public class DoctorManager : IDoctorManager
{
    private readonly IMapper _mapper;
    private readonly IValidation<Doctor> _validation;
    private readonly IBaseRepository<Doctor> _repository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDoctorSearchCache _doctorSearchCache;
    
    public DoctorManager(IBaseRepository<Doctor> repository, IMapper mapper, IValidation<Doctor> validation, IDoctorRepository doctorRepository, BL.hospital.Caching.IDoctorSearchCache doctorSearchCache)
    {
        _repository = repository;
        _mapper = mapper;
        _validation = validation;
        _doctorRepository = doctorRepository;
        _doctorSearchCache = doctorSearchCache;
    }

    public async Task<DoctorDto?> GetById(Guid id)
    {
        var doctor =  await _repository.ReadById(id);
        if (doctor == null)
        {
            throw new KeyNotFoundException($"Doctor with ID {id} not found.");
        }
        
        return _mapper.Map<DoctorDto>(doctor);
    }
    
    public async Task<Doctor?> GetDoctorById(Guid id)
    {
        var doctor =  await _repository.ReadById(id);
        if (doctor == null)
        {
            throw new KeyNotFoundException($"Doctor with ID {id} not found.");
        }

        return doctor;
    }

    public async Task<IEnumerable<DoctorDto>> GetAll()
    {
        var doctors = await _repository.ReadAll();
        return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
    }

    public async Task<Doctor> Add(AddDoctorDto patient)
    {
        Doctor doctor = _mapper.Map<Doctor>(patient);
        var validationResults = _validation.Validate(doctor).ToList();
        if (validationResults.Any())
        {
            var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
            throw new ValidationException(errors);
        }
        
        return await _repository.Create(doctor);
        
    }

    public void Remove(Guid id)
    {
        _repository.Delete(id);
    }

    public async Task<IEnumerable<DoctorDto>> SearchByFullNameOrSpecialisation(string? term, CancellationToken cancellationToken = default)
    {
        // Cache only for non-empty terms.
        var cached = await _doctorSearchCache.TryGet(term, cancellationToken);
        if (cached is not null)
        {
            return cached;
        }

        var doctors = await _doctorRepository.SearchDoctorsByFullNameOrSpecialisation(term, cancellationToken);
        var mapped = _mapper.Map<IReadOnlyList<DoctorDto>>(doctors);

        // Keep it short to avoid stale suggestions.
        if (!string.IsNullOrWhiteSpace(term))
        {
            await _doctorSearchCache.Set(term, mapped, ttl: TimeSpan.FromMinutes(2), cancellationToken);
        }

        return mapped;
    }
}