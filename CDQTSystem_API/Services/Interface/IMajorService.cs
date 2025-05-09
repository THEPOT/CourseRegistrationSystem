using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Interface
{
    public interface IMajorService
    {
        Task<List<MajorResponse>> GetAllMajors();
        Task<MajorResponse> GetMajorById(Guid id);
        Task<MajorResponse> CreateMajor(MajorCreateRequest request);
        Task<MajorResponse> UpdateMajor(Guid id, MajorUpdateRequest request);
        Task<bool> DeleteMajor(Guid id);
    }
} 