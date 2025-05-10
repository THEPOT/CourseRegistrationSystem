using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_Domain.Paginate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Interface
{
    public interface IMajorService
    {
        Task<IPaginate<MajorResponse>> GetAllMajors(int page, int size, string? search);
		Task<MajorResponse> GetMajorById(Guid id);
        Task<MajorResponse> CreateMajor(MajorCreateRequest request);
        Task<MajorResponse> UpdateMajor(Guid id, MajorUpdateRequest request);
        Task<bool> DeleteMajor(Guid id);
    }
} 