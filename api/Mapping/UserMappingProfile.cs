using AutoMapper;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterRequest, User>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToLower()))
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());
    }
}
