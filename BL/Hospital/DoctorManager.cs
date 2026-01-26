using System.ComponentModel.DataAnnotations;
using AutoMapper;
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
    
    public DoctorManager(IBaseRepository<Doctor> repository, IMapper mapper, IValidation<Doctor> validation, IDoctorRepository doctorRepository)
    {
        _repository = repository;
        _mapper = mapper;
        _validation = validation;
        _doctorRepository = doctorRepository;
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
        var doctors = await _doctorRepository.SearchDoctorsByFullNameOrSpecialisation(term, cancellationToken);
        return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
    }
}