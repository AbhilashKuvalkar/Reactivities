using Application.Activities.DTOs;
using AutoMapper;

namespace Application.Core;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Domain.Activity, Domain.Activity>();
        CreateMap<CreateActivityDto, Domain.Activity>();
        CreateMap<EditActivityDto, Domain.Activity>();
    }
}
