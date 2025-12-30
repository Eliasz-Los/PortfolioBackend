using AutoMapper;
using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital.mapper;

public class AppointmentMappingProfile : Profile
{
    public AppointmentMappingProfile()
    {
        CreateMap<AddAppointmentDto, Appointment>();
        CreateMap<Appointment, AddAppointmentDto>();
    }
}