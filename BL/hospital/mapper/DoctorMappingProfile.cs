using AutoMapper;
using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital.mapper;

public class DoctorMappingProfile : Profile
{
    public DoctorMappingProfile()
    {
        CreateMap<Doctor, AddDoctorDto>();
        CreateMap<AddDoctorDto, Doctor>();
        
        CreateMap<Doctor, DoctorDto>();
        CreateMap<DoctorDto, Doctor>();
    }
}