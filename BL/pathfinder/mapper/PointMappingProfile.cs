using AutoMapper;
using BL.dto;
using Domain.pathfinder;

namespace BL.mapper;

public class PointMappingProfile : Profile
{
    public PointMappingProfile()
    {
   
        CreateMap<Point, PathPointDto>();
        CreateMap<PathPointDto, Point>();
    }
}