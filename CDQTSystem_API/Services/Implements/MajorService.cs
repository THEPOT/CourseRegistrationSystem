using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Implements
{
    public class MajorService : IMajorService
    {
        private readonly IUnitOfWork<DbContext> _unitOfWork;
        public MajorService(IUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MajorResponse>> GetAllMajors()
        {
            var majors = await _unitOfWork.GetRepository<Major>().GetListAsync();
            return majors.Select(m => new MajorResponse
            {
                Id = m.Id,
                MajorName = m.MajorName,
                RequiredCredits = m.RequiredCredits
            }).ToList();
        }

        public async Task<MajorResponse> GetMajorById(Guid id)
        {
            var m = await _unitOfWork.GetRepository<Major>().SingleOrDefaultAsync( predicate: x => x.Id == id);
            if (m == null) return null;
            return new MajorResponse
            {
                Id = m.Id,
                MajorName = m.MajorName,
                RequiredCredits = m.RequiredCredits
            };
        }

        public async Task<MajorResponse> CreateMajor(MajorCreateRequest request)
        {
            var major = new Major
            {
                Id = Guid.NewGuid(),
                MajorName = request.MajorName,
                RequiredCredits = request.RequiredCredits
            };
            await _unitOfWork.GetRepository<Major>().InsertAsync(major);
            await _unitOfWork.CommitAsync();
            return new MajorResponse
            {
                Id = major.Id,
                MajorName = major.MajorName,
                RequiredCredits = major.RequiredCredits
            };
        }

        public async Task<MajorResponse> UpdateMajor(Guid id, MajorUpdateRequest request)
        {
            var repo = _unitOfWork.GetRepository<Major>();
            var major = await repo.SingleOrDefaultAsync( predicate:x => x.Id == id);
            if (major == null) return null;
            major.MajorName = request.MajorName;
            major.RequiredCredits = request.RequiredCredits;
            repo.UpdateAsync(major);
            await _unitOfWork.CommitAsync();
            return new MajorResponse
            {
                Id = major.Id,
                MajorName = major.MajorName,
                RequiredCredits = major.RequiredCredits
            };
        }

        public async Task<bool> DeleteMajor(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Major>();
            var major = await repo.SingleOrDefaultAsync( predicate:x => x.Id == id);
            if (major == null) return false;
            repo.DeleteAsync(major);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
} 