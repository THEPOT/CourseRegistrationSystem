using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CDQTSystem_API.Services.Implements
{
    public class RegistrationPeriodService : BaseService<RegistrationPeriodService>, IRegistrationPeriodService
    {
        public RegistrationPeriodService(IUnitOfWork<DbContext> unitOfWork, ILogger<RegistrationPeriodService> logger,
                          IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<RegistrationPeriodResponse> CreateRegistrationPeriod(RegistrationPeriodCreateRequest request, Guid userId)
        {
            try
            {
                // Validate semester exists
                var semester = await _unitOfWork.GetRepository<Semester>()
                    .SingleOrDefaultAsync(predicate: s => s.Id == request.SemesterId);

                if (semester == null)
                    throw new BadHttpRequestException("Semester not found");

                var startDate = request.StartDate;
                var endDate = request.EndDate;

                // Validate dates
                if (startDate >= endDate)
                    throw new BadHttpRequestException("Start date must be before end date");


                // Create new registration period
                var registrationPeriod = new RegistrationPeriod
                {
                    Id = Guid.NewGuid(),
                    SemesterId = request.SemesterId,
                    MaxCredits = request.MaxCredits,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = "CLOSED",
                    CreatedBy = userId,
                    CreatedDate = DateTime.Now
                };

                await _unitOfWork.GetRepository<RegistrationPeriod>().InsertAsync(registrationPeriod);
                await _unitOfWork.CommitAsync();

                // Return response
                return new RegistrationPeriodResponse
                {
                    Id = registrationPeriod.Id,
                    SemesterId = registrationPeriod.SemesterId,
                    SemesterName = semester.SemesterName,
                    StartDate = registrationPeriod.StartDate,
                    EndDate = registrationPeriod.EndDate,
                    Status = registrationPeriod.Status,
                    MaxCredits = registrationPeriod.MaxCredits
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating registration period: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<RegistrationPeriodResponse> GetRegistrationPeriodById(Guid id)
        {
            var registrationPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
                .SingleOrDefaultAsync(
                    predicate: rp => rp.Id == id,
                    include: q => q.Include(rp => rp.Semester)
                );

            if (registrationPeriod == null)
                return null;

            return new RegistrationPeriodResponse
            {
                Id = registrationPeriod.Id,
                SemesterId = registrationPeriod.SemesterId,
                SemesterName = registrationPeriod.Semester.SemesterName,
                StartDate = registrationPeriod.StartDate,
                EndDate = registrationPeriod.EndDate,
                Status = registrationPeriod.Status
            };
        }

        public Task<RegistrationPeriodResponse> GetCurrentRegistrationPeriod()
        {
            throw new NotImplementedException();
        }

        public Task<List<ProgramRegistrationStatisticsResponse>> GetProgramStatistics(Guid periodId)
        {
            throw new NotImplementedException();
        }

        public Task<RegistrationStatisticsResponse> GetRegistrationStatistics(Guid periodId, Guid? programId = null, Guid? courseId = null)
        {
            throw new NotImplementedException();
        }

        public Task<List<RegistrationSummaryResponse>> GetRegistrationSummary(Guid termId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateRegistrationPeriodStatus(Guid periodId, string status)
        {
            try
            {
                var registrationPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
                    .SingleOrDefaultAsync(predicate: rp => rp.Id == periodId);

                if (registrationPeriod == null)
                    throw new BadHttpRequestException("Registration period not found");

                // Validate status
                var validStatuses = new[] { "MAINTENANCE", "OPEN", "CLOSED" };
                if (!validStatuses.Contains(status))
                    throw new BadHttpRequestException($"Invalid status. Valid statuses: {string.Join(", ", validStatuses)}");

                // Additional validation rules
                if (status == "OPEN")
                {
                    // Check if there's already an open registration period
                    var existingOpenPeriod = await _unitOfWork.GetRepository<RegistrationPeriod>()
                        .SingleOrDefaultAsync(predicate: rp => 
                            rp.Status == "OPEN" && 
                            rp.Id != periodId &&
                            rp.SemesterId == registrationPeriod.SemesterId);

                    if (existingOpenPeriod != null)
                        throw new BadHttpRequestException("Another registration period is already open for this semester");

                    // Validate dates
                    if (DateTime.UtcNow > registrationPeriod.EndDate)
                        throw new BadHttpRequestException("Cannot open a registration period that has already ended");
                }

                registrationPeriod.Status = status;
                _unitOfWork.GetRepository<RegistrationPeriod>().UpdateAsync(registrationPeriod);
                await _unitOfWork.CommitAsync();
                _logger.LogInformation("Status updated successfully to: {Status}", registrationPeriod.Status);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating registration period status: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<List<RegistrationPeriodResponse>> GetAllRegistrationPeriods()
        {
            try
            {
                var registrationPeriods = await _unitOfWork.GetRepository<RegistrationPeriod>()
                    .GetListAsync(
                        include: q => q
                            .Include(rp => rp.Semester)
                            .Include(rp => rp.CourseRegistrations),
                        orderBy: q => q
                            .OrderByDescending(rp => rp.StartDate)
                    );

                return registrationPeriods.Select(rp => new RegistrationPeriodResponse
                {
                    Id = rp.Id,
                    SemesterId = rp.SemesterId,
                    SemesterName = rp.Semester.SemesterName,
                    StartDate = rp.StartDate,
                    EndDate = rp.EndDate,
                    Status = rp.Status,
                    MaxCredits = rp.MaxCredits,
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all registration periods");
                throw;
            }
        }

    }
}






