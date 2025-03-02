using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;


namespace CourseRegistration_API.Services.Interface
{
	public interface IUsersService
	{
		Task<LoginResponse> Login(LoginRequest loginRequest);

		Task<RegisterResponse> Register(RegisterRequest registerRequest);
	}
}
