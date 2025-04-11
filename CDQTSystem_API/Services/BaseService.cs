using AutoMapper;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CDQTSystem_API.Services
{
	public abstract class BaseService<T> where T : class
	{
		protected IUnitOfWork<DbContext> _unitOfWork;
		protected ILogger<T> _logger;
		protected IMapper _mapper;
		protected IHttpContextAccessor _httpContextAccessor;

		public BaseService(IUnitOfWork<DbContext> unitOfWork, ILogger<T> logger, IMapper mapper,
			IHttpContextAccessor httpContextAccessor)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}

		protected string GetUsernameFromJwt()
		{
			string username = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
			return username;
		}

		protected string GetRoleFromJwt()
		{
			string role = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
			return role;
		}

		protected string GetBrandIdFromJwt()
		{
			return _httpContextAccessor?.HttpContext?.User?.FindFirstValue("brandId");
		}

		protected string GetStoreIdFromJwt()
		{
			return _httpContextAccessor?.HttpContext?.User?.FindFirstValue("storeId");
		}
	}
}
