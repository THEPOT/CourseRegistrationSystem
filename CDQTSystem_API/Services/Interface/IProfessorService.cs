using CDQTSystem_API.Payload.Response;

namespace CDQTSystem_API.Services.Interface
{
    public interface IProfessorService
    {
        Task<List<ProfessorResponse>> GetAllProfessors();
    }
}