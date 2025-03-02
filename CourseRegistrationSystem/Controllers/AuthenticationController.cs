﻿
using Azure.Messaging;
using CourseRegistration_API.Constants;
using CourseRegistration_API.Payload.Response;

using Microsoft.AspNetCore.Mvc;
using CourseRegistration_API.Enums;
using CourseRegistration_API.Services.Interface;
using CourseRegistration_API.Payload.Request;

namespace CourseRegistration_API.Controllers
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

	}
}
