using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;


namespace CDQTSystem_API.Services.Interface
{
	public interface IUsersService
	{
		Task<LoginResponse> Login(LoginRequest loginRequest);

		Task<RegisterResponse> Register(RegisterRequest registerRequest);

		Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
	}
}
