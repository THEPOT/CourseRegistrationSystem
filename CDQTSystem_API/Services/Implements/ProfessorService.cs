using AutoMapper;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
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

        public async Task<List<ProfessorResponse>> GetAllProfessors()
        {
            try
            {
                var professors = await _unitOfWork.GetRepository<Professor>()
                    .GetListAsync(
                        include: q => q
                            .Include(p => p.User)
                            .Include(p => p.Department),
                        orderBy: q => q.OrderBy(p => p.User.FullName)
                    );

                return professors.Select(p => new ProfessorResponse
                {
                    Id = p.Id,
                    FullName = p.User.FullName,
                    Email = p.User.Email,
                    DepartmentId = p.DepartmentId,
                    DepartmentName = p.Department.DepartmentName,
                    ImageUrl = p.User.Image
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all professors");
                throw;
            }
        }
    }
}