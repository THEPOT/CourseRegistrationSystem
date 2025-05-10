using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;

namespace CDQTSystem_API.Services.Interface
{
    public interface IProfessorService
    {
        Task<IPaginate<ProfessorResponse>> GetAllProfessors(int page, int size, string? search);
        Task<List<ProfessorScheduleResponse>> GetProfessorSchedule(Guid userId,int? year , int? week);
	}
}