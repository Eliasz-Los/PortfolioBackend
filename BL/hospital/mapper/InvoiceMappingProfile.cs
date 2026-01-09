using AutoMapper;
using BL.hospital.dto;
using Domain.hospital;

namespace BL.hospital.mapper;

public class InvoiceMappingProfile : Profile
{
    public InvoiceMappingProfile()
    {
        CreateMap<Invoice, InvoiceDto>();
        CreateMap<InvoiceDto, Invoice>();
    }
}