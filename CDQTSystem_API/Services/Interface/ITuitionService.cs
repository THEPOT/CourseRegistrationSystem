using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Interface
{
    public interface ITuitionService
    {
        Task<TuitionPeriodResponse> CreateTuitionPeriod(TuitionPeriodCreateRequest request);
        Task<bool> UpdateTuitionPeriodStatus(Guid periodId, string status);
        Task<bool> RecordTuitionPayment(TuitionPaymentRequest request);
        Task<TuitionStatusResponse> GetStudentTuitionStatus(Guid studentId, Guid semesterId);
        Task<List<TuitionStudentSummary>> GetTuitionSummary(Guid termId);
        Task<TuitionPeriodResponse> GetCurrentTuitionPeriod();
        Task<List<TuitionRateResponse>> GetTuitionRates();
        Task<bool> UpdateTuitionRates(List<TuitionRateRequest> rates);
        Task<List<ProgramRateResponse>> GetProgramRates();
        Task<bool> UpdateProgramRates(List<ProgramRateRequest> rates);
        Task<List<TuitionPaymentResponse>> GetTuitionPayments(TuitionPaymentFilterRequest filter);
        Task<TuitionStatisticsResponse> GetTuitionStatistics();
    }
}
