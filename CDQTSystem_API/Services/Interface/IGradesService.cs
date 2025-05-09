using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
    public interface IGradesService
    {
         Task<GradeResponse> CreateOrUpdateGrade(GradeEntryRequest request);
    }
}
