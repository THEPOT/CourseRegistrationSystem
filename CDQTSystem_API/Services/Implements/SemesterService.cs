using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Domain.Paginate;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
    public class SemesterService : BaseService<SemesterService>, ISemesterService
    {
        public SemesterService(IUnitOfWork<DbContext> unitOfWork, ILogger<SemesterService> logger,
                          IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<SemesterResponse> CreateSemester(SemesterCreateRequest request)
        {
            try
            {
                var semester = new Semester
                {
                    Id = Guid.NewGuid(),
                    SemesterName = request.Name,
                    StartDate = DateOnly.Parse(request.StartDate),
                    EndDate = DateOnly.Parse(request.EndDate),
                    AcademicYear = request.AcademicYear,
                    Status = request.Status
                };

                await _unitOfWork.GetRepository<Semester>().InsertAsync(semester);
                await _unitOfWork.CommitAsync();

                return _mapper.Map<SemesterResponse>(semester);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating semester");
                throw;
            }
        }

		public async Task<IPaginate<SemesterResponse>> GetAllSemesters(int page, int size, string? search)
		{
			IPaginate<SemesterResponse> semestersResponse = await _unitOfWork.GetRepository<Semester>()
				.GetPagingListAsync(
					selector: s => new SemesterResponse
					{
						Id = s.Id,
						SemesterName = s.SemesterName,
						StartDate = s.StartDate,
						AcademicYear = s.AcademicYear,
						EndDate = s.EndDate,
						Status = s.Status
					},
					predicate: s => string.IsNullOrEmpty(search) || s.SemesterName.Contains(search),
					orderBy: q => q.OrderByDescending(s => s.StartDate),
					page: page,
					size: size
				);

			return semestersResponse;
		}

		public async Task<SemesterResponse> GetSemesterById(Guid id)
        {
            var semester = await _unitOfWork.GetRepository<Semester>()
                .SingleOrDefaultAsync(predicate: s => s.Id == id);
            return _mapper.Map<SemesterResponse>(semester);
        }

        public async Task<bool> UpdateSemesterStatus(Guid id, string status)
        {
            var semester = await _unitOfWork.GetRepository<Semester>()
                .SingleOrDefaultAsync(predicate: s => s.Id == id);

            if (semester == null)
                throw new BadHttpRequestException("Semester not found");

            semester.Status = status;
            await _unitOfWork.CommitAsync();

            return true;
        }
    }
}