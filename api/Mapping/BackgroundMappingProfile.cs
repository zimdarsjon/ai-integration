using AutoMapper;

public class BackgroundMappingProfile : Profile
{
    public BackgroundMappingProfile()
    {
        CreateMap<Background, BackgroundSummaryDto>();

        CreateMap<Background, BackgroundDetailDto>()
            .ForMember(d => d.SkillProficiencies, o => o.MapFrom(s => s.SkillProficiencies.Select(sp => sp.SkillName).ToList()));

        CreateMap<BackgroundFeature, BackgroundFeatureDto>();
    }
}
