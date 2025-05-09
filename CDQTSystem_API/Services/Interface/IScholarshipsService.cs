using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Interface
{
    public interface IScholarshipsService
    {
        Task<PaginatedScholarshipResponse> GetScholarships(int page, int size);
        Task<ScholarshipResponse> CreateScholarship(ScholarshipRequest request);
        Task<ScholarshipResponse> UpdateScholarship(Guid id, ScholarshipRequest request);
        Task<bool> DeleteScholarship(Guid id);
        Task<List<ScholarshipRecipientResponse>> GetScholarshipRecipients(Guid scholarshipId);
        Task<ScholarshipReviewResponse> ReviewScholarships(Guid termId);
    }
} 