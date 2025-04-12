using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
    public interface IRegistrationPeriodService
    {
        Task<RegistrationPeriodResponse> CreateRegistrationPeriod(RegistrationPeriodCreateRequest request, Guid userId);
        Task<List<RegistrationPeriodResponse>> GetAllRegistrationPeriods();
        Task<bool> UpdateRegistrationPeriodStatus(Guid periodId, string status);
        Task<RegistrationPeriodResponse> GetCurrentRegistrationPeriod();
        Task<List<RegistrationSummaryResponse>> GetRegistrationSummary(Guid termId);
        Task<RegistrationStatisticsResponse> GetRegistrationStatistics(Guid periodId, Guid? programId = null, Guid? courseId = null);
        Task<List<ProgramRegistrationStatisticsResponse>> GetProgramStatistics(Guid periodId);
        Task<RegistrationPeriodResponse> GetRegistrationPeriodById(Guid id);
    }
}