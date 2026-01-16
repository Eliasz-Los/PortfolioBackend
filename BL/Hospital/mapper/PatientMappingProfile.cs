using AutoMapper;
using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital.mapper;

public class PatientMappingProfile : Profile
{
    public PatientMappingProfile()
    {
        CreateMap<Patient, AddPatientDto>();
        CreateMap<AddPatientDto, Patient>();
        
        CreateMap<Patient, PatientDto>();
        CreateMap<PatientDto, Patient>();
    }
    
}