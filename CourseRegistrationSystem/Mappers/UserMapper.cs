using AutoMapper;
using CourseRegistration_API.Enums;
using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Utils;
using CourseRegistration_Domain.Entities;
namespace CourseRegistration_API.Mappers
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
