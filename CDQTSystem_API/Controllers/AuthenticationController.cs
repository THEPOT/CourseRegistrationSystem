
using Azure.Messaging;
using CDQTSystem_API.Constants;
using CDQTSystem_API.Payload.Response;

using Microsoft.AspNetCore.Mvc;
using CDQTSystem_API.Enums;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_API.Payload.Request;

namespace CDQTSystem_API.Controllers
{
	public class AuthenticationController : BaseController<AuthenticationController>
	{
		private readonly IUsersService _usersService;
		public AuthenticationController(ILogger<AuthenticationController> logger, IUsersService usersService) : base(logger)
		{
			_usersService = usersService;
		}
	
		[HttpPost(ApiEndPointConstant.Authentication.Login)]
		[ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
		[ProducesErrorResponseType(typeof(UnauthorizedObjectResult))]
		public async Task<IActionResult> Login(LoginRequest loginRequest)
		{
			var loginResponse = await _usersService.Login(loginRequest);
			if (loginResponse == null)
			{
				return Unauthorized(new ErrorResponse()
				{
					StatusCode = StatusCodes.Status401Unauthorized,
					Error = MessageConstant.LoginMessage.InvalidUsernameOrPassword,
					TimeStamp = DateTime.Now
				});
			}
			return Ok(loginResponse);
		}


		[HttpPost(ApiEndPointConstant.Authentication.Register)]
		[ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
		[ProducesErrorResponseType(typeof(UnauthorizedObjectResult))]
		public async Task<IActionResult> Register(RegisterRequest registerRequest)
		{
			var registerResponse = await _usersService.Register(registerRequest);
			if (registerResponse == null)
			{
				return Unauthorized(new ErrorResponse()
				{
					StatusCode = StatusCodes.Status401Unauthorized,
					Error = MessageConstant.RegisterMessage.RegisterFailed,
					TimeStamp = DateTime.Now
				});
			}
			return Ok(registerResponse);
		}

		[HttpPost(ApiEndPointConstant.Authentication.RefreshToken)]
		[ProducesResponseType(typeof(RefreshTokenResponse), StatusCodes.Status200OK)]
		[ProducesErrorResponseType(typeof(UnauthorizedObjectResult))]
		public async Task<IActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
		{
			var refreshTokenResponse = await _usersService.RefreshToken(refreshTokenRequest);
			if (refreshTokenResponse == null)
			{
				return Unauthorized(new ErrorResponse()
				{
					StatusCode = StatusCodes.Status401Unauthorized,
					Error = MessageConstant.RefreshTokenMessage.RefreshTokenFailed,
					TimeStamp = DateTime.Now
				});
			}
			return Ok(refreshTokenResponse);
		}

	}
}
