using AutoMapper;
using CDQTSystem_API.Enums;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Utils;
using CDQTSystem_Domain.Entities;
namespace CDQTSystem_API.Mappers
{
	public class UserMapper : Profile
	{
		public UserMapper()
		{
			CreateMap<LoginRequest, User>().ReverseMap();
			CreateMap<RegisterRequest, User>().ReverseMap();
			CreateMap<User, LoginResponse>().ReverseMap();

			CreateMap<RegisterRequest, User>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
				.ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.ImageUrl))
				.ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Role));

			CreateMap<User, RegisterResponse>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Image))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src =>
					src.Role != null ? EnumUtil.ParseEnum<RoleEnum>(src.Role.RoleName) : RoleEnum.Student));
		}
	}
}
