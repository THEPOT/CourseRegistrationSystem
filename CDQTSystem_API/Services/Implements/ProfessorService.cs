using AutoMapper;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Domain.Paginate;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
    public class ProfessorService : BaseService<ProfessorService>, IProfessorService
    {
        public ProfessorService(IUnitOfWork<DbContext> unitOfWork, 
            ILogger<ProfessorService> logger,
            IMapper mapper, 
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<IPaginate<ProfessorResponse>> GetAllProfessors(int page, int size, string? search)
        {
            try
            {
                IPaginate<ProfessorResponse> professors = await _unitOfWork.GetRepository<Professor>()
                    .GetPagingListAsync(
					selector: p => new ProfessorResponse
					{
						Id = p.Id,
						FullName = p.User.FullName,
						Email = p.User.Email,
						PhoneNumber = p.User.PhoneNumber,
						DepartmentName = p.Department.DepartmentName,
					},
					predicate: p => string.IsNullOrEmpty(search) || p.User.FullName.Contains(search),
						include: q => q
                            .Include(p => p.User)
                            .Include(p => p.Department),
                        orderBy: q => q.OrderBy(p => p.User.FullName)
                    );
				return professors;
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all professors");
                throw;
            }
        }
    }
}