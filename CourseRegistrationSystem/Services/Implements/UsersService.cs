using AutoMapper;
using CourseRegistration_API.Constants;
using CourseRegistration_API.Enums;
using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using CourseRegistration_API.Utils;
using CourseRegistration_Domain.Entities;
using CourseRegistration_Repository.Interfaces;
using CourseRegistration_API.Helpers;

using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CourseRegistration_API.Services.Implements
{
	public class UsersService : BaseService<UsersService>, IUsersService
	{
		public UsersService(IUnitOfWork<DbContext> unitOfWork, ILogger<UsersService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<LoginResponse> Login(LoginRequest loginRequest)
		{
			Expression<Func<User, bool>> searchFilter = p => p.Email.Equals(loginRequest.Email) && p.Password.Equals(PasswordUtil.HashPassword(loginRequest.Password));
			User account = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: searchFilter, include: u => u.Include(u => u.Role));

			if (account == null) return null;
			RoleEnum role = EnumUtil.ParseEnum<RoleEnum>(account.Role.RoleName);
			Tuple<string, Guid> guidClaim = null;
			LoginResponse loginResponse = null;
			switch (role)
			{
				case RoleEnum.Admin:
				case RoleEnum.Staff:
					Guid staffId = await _unitOfWork.GetRepository<Staff>().SingleOrDefaultAsync(selector: x => x.Id, predicate: x => x.UserId.Equals(account.Id));
					guidClaim = new Tuple<string, Guid>("staffId", staffId);
					loginResponse = new LoginResponse(
						 account.FullName,
					 account.Role.RoleName,
						account.Image
					);
					break;
				case RoleEnum.Student:
					Guid studentId = await _unitOfWork.GetRepository<Student>().SingleOrDefaultAsync(selector: x => x.Id, predicate: x => x.UserId.Equals(account.Id));
					guidClaim = new Tuple<string, Guid>("studentId", studentId);
					loginResponse = new LoginResponse(
					 account.FullName,
						 account.Role.RoleName,
						 account.Image
					);
					break;
				case RoleEnum.Lecturer:
					Guid lecturerId = await _unitOfWork.GetRepository<Lecturer>().SingleOrDefaultAsync(selector: x => x.Id, predicate: x => x.UserId.Equals(account.Id));
					guidClaim = new Tuple<string, Guid>("lecturerId", lecturerId);
					loginResponse = new LoginResponse(
						 account.FullName,
						 account.Role.RoleName,
						 account.Image
					);
					break;
				default:
					new LoginResponse(
					   account.FullName,
					   account.Role.RoleName,
					   account.Image
					   );
					break;
			}
			var token = JwtUtil.GenerateJwtToken(account, guidClaim);
			var refreshToken = JwtUtil.GenerateRefreshToken(account, guidClaim);
			loginResponse.AccessToken = token;
			loginResponse.RefreshToken = refreshToken;
			return loginResponse;
		}

		public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
		{
			try
			{
				Expression<Func<User, bool>> emailFilter = p => p.Email.Equals(registerRequest.Email);
				var existingUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: emailFilter);
				if (existingUser != null) throw new BadHttpRequestException(MessageConstant.RegisterMessage.EmailExisted);
				Expression<Func<Role, bool>> roleFilter = r => r.RoleName.ToLower() == registerRequest.Role.ToString().ToLower();
				var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: roleFilter);
				if (role == null) throw new BadHttpRequestException(MessageConstant.RegisterMessage.RoleNotFound);

				var newUser = new User
				{
					Id = Guid.NewGuid(),
					Email = registerRequest.Email,
					Password = PasswordUtil.HashPassword(registerRequest.Password),
					FullName = registerRequest.FullName,
					RoleId = role.Id,
					Image = registerRequest.ImageUrl
				};

				await _unitOfWork.GetRepository<User>().InsertAsync(newUser);
				RoleEnum roleEnum = EnumUtil.ParseEnum<RoleEnum>(registerRequest.Role.ToString());

				switch (roleEnum)
				{
					case RoleEnum.Student:
						var newStudent = new Student
						{
							Id = Guid.NewGuid(),
							UserId = newUser.Id,
							ProgramId = registerRequest.ProgramId,
							Mssv = MSSVGeneration.GenerateStudentId(),
							EnrollmentDate = DateOnly.FromDateTime(DateTime.Now) // Set to current date
						};
						await _unitOfWork.GetRepository<Student>().InsertAsync(newStudent);
						break;

					case RoleEnum.Lecturer:
						var newLecturer = new Lecturer
						{
							Id = Guid.NewGuid(),
							UserId = newUser.Id,
						};
						await _unitOfWork.GetRepository<Lecturer>().InsertAsync(newLecturer);
						break;
					case RoleEnum.Admin:
					case RoleEnum.Staff:
						var newStaff = new Staff
						{
							Id = Guid.NewGuid(),
							UserId = newUser.Id,
						};
						await _unitOfWork.GetRepository<Staff>().InsertAsync(newStaff);
						break;
					default:
						_logger.LogWarning("Unhandled role {RoleName} during registration.", registerRequest.Role);
						return null;
				}
				bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
				if (isSuccessful)
				{
					// Reload the user with role included
					var userWithRole = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
						predicate: u => u.Id == newUser.Id,
						include: q => q.Include(u => u.Role)
					);

					return _mapper.Map<RegisterResponse>(userWithRole);
				}
				return null;
			}
			catch (Exception ex)
			{
				// Log full exception details
				_logger.LogError(ex, "Registration failed with inner exception: {Message}", ex.InnerException?.Message);
				throw; // Or handle differently
			}
		}

	}
}
