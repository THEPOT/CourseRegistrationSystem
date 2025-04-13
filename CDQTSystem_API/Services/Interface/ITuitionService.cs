using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
    public interface ITuitionService
    {
        Task<TuitionPeriodResponse> CreateTuitionPeriod(TuitionPeriodCreateRequest request);
        Task<bool> UpdateTuitionPeriodStatus(Guid periodId, string status);
        Task<bool> RecordTuitionPayment(TuitionPaymentRequest request);
        Task<TuitionStatusResponse> GetStudentTuitionStatus(Guid studentId, Guid semesterId);
        Task<List<TuitionSummaryResponse>> GetTuitionSummary(Guid termId);
        Task<TuitionPeriodResponse> GetCurrentTuitionPeriod();
    }
}
