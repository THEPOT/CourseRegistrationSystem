using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
	public class DepartmentService : BaseService<DepartmentService>, IDepartmentService
	{
		public DepartmentService(IUnitOfWork<DbContext> unitOfWork, ILogger<DepartmentService> logger,
					  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public Task<DepartmentResponse> CreateDepartment(DepartmentCreateRequest request)
		{
			var department = _mapper.Map<Department>(request);
			_unitOfWork.GetRepository<Department>().InsertAsync(department);
			_unitOfWork.CommitAsync();
			var departmentResponse = _mapper.Map<DepartmentResponse>(department);
			return Task.FromResult(departmentResponse);
		}

		public Task<List<DepartmentResponse>> GetAllDepartments()
		{
			var departments = _unitOfWork.GetRepository<Department>().GetListAsync();
			var departmentResponses = _mapper.Map<List<DepartmentResponse>>(departments);
			return Task.FromResult(departmentResponses);
		}

		public Task<DepartmentResponse> GetDepartmentById(Guid departmentId)
		{
			var department = _unitOfWork.GetRepository<Department>().SingleOrDefaultAsync(
				predicate: d => d.Id == departmentId
			).Result;
			if (department == null)
				return Task.FromResult<DepartmentResponse>(null);
			var departmentResponse = _mapper.Map<DepartmentResponse>(department);
			return Task.FromResult(departmentResponse);

		}

		public Task<DepartmentResponse> UpdateDepartment(Guid departmentId, DepartmentUpdateRequest request)
		{
			var department = _unitOfWork.GetRepository<Department>().SingleOrDefaultAsync(
				predicate: d => d.Id == departmentId
			).Result;
			if (department == null)
				return Task.FromResult<DepartmentResponse>(null);
			_mapper.Map(request, department);
			_unitOfWork.GetRepository<Department>().UpdateAsync(department);
			_unitOfWork.CommitAsync();
			var departmentResponse = _mapper.Map<DepartmentResponse>(department);
			return Task.FromResult(departmentResponse);

		}
	}
}
