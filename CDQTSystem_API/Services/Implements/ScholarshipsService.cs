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
    public class ScholarshipsService : IScholarshipsService
    {
        private readonly IUnitOfWork<DbContext> _unitOfWork;
        public ScholarshipsService(IUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

		public async Task<PaginatedScholarshipResponse> GetScholarships(int page, int size)
		{
			var repo = _unitOfWork.GetRepository<Scholarship>();
			var total = await repo.GetListAsync();
			var totalCount = total.Count;

			var items = await repo.GetPagingListAsync(
				selector: s => new ScholarshipResponse
				{
					Id = s.Id,
					ScholarshipName = s.ScholarshipName,
					Description = s.Description,
					Amount = s.Amount,
					EligibilityCriteria = s.EligibilityCriteria,
					DepartmentId = s.DepartmentId.HasValue ? s.DepartmentId.Value : Guid.Empty,
					DepartmentName = s.Department != null ? s.Department.DepartmentName : string.Empty
				},
				orderBy: q => q.OrderBy(s => s.ScholarshipName),
				page: page,
				size: size,
				include: q => q.Include(s => s.Department)
			);

			return new PaginatedScholarshipResponse
			{
				Total = totalCount,
				Page = page,
				Size = size,
				Items = items.Items.ToList()
			};
		}

        public async Task<ScholarshipResponse> CreateScholarship(ScholarshipRequest request)
        {
            var scholarship = new Scholarship
            {
                Id = Guid.NewGuid(),
                ScholarshipName = request.ScholarshipName,
                Description = request.Description,
                Amount = request.Amount,
                EligibilityCriteria = request.EligibilityCriteria,
                DepartmentId = request.DepartmentId
            };
            await _unitOfWork.GetRepository<Scholarship>().InsertAsync(scholarship);
            await _unitOfWork.CommitAsync();
            return await MapToResponse(scholarship.Id);
        }

        public async Task<ScholarshipResponse> UpdateScholarship(Guid id, ScholarshipRequest request)
        {
            var repo = _unitOfWork.GetRepository<Scholarship>();
            var scholarship = await repo.SingleOrDefaultAsync( predicate: s => s.Id == id);
            if (scholarship == null) return null;
            scholarship.ScholarshipName = request.ScholarshipName;
            scholarship.Description = request.Description;
            scholarship.Amount = request.Amount;
            scholarship.EligibilityCriteria = request.EligibilityCriteria;
            scholarship.DepartmentId = request.DepartmentId;
            repo.UpdateAsync(scholarship);
            await _unitOfWork.CommitAsync();
            return await MapToResponse(scholarship.Id);
        }

        public async Task<bool> DeleteScholarship(Guid id)
        {
            var repo = _unitOfWork.GetRepository<Scholarship>();
            var scholarship = await repo.SingleOrDefaultAsync( predicate: s => s.Id == id);
            if (scholarship == null) return false;
             repo.DeleteAsync(scholarship);
            await _unitOfWork.CommitAsync();
            return true;
        }

		public async Task<List<ScholarshipRecipientResponse>> GetScholarshipRecipients(Guid scholarshipId)
		{
			var recipients = await _unitOfWork.GetRepository<StudentScholarship>()
				.GetListAsync(
					predicate: ss => ss.ScholarshipId == scholarshipId,
					include: q => q.Include(ss => ss.Student).ThenInclude(s => s.User)
				);
			return recipients.Select(ss => new ScholarshipRecipientResponse
			{
				StudentId = ss.StudentId,
				StudentName = ss.Student.User.FullName,
				Mssv = ss.Student.User.UserCode,
				AwardDate = ss.AwardDate.ToDateTime(TimeOnly.MinValue),
				Amount = ss.Scholarship.Amount
			}).ToList();
		}

        public async Task<ScholarshipReviewResponse> ReviewScholarships(Guid termId)
        {

            var students = await _unitOfWork.GetRepository<Student>()
                .GetListAsync(
                    include: q => q.Include(s => s.User)
                        .Include(s => s.CourseRegistrations)
                        .ThenInclude(r => r.ClassSection)
                        .ThenInclude(cs => cs.Semester)
                        .Include(s => s.StudentScholarships)
                );
            var awarded = new List<ScholarshipRecipientResponse>();
            var notAwarded = new List<ScholarshipRecipientResponse>();
            foreach (var student in students)
            {
                var gpa = 0m;
                var totalCredits = 0;
                foreach (var reg in student.CourseRegistrations.Where(r => r.ClassSection.SemesterId == termId))
                {
                    if (reg.Grades != null && reg.Grades.Any())
                    {
                        var grade = reg.Grades.FirstOrDefault();
                        gpa += (grade?.QualityPoints ?? 0) * reg.ClassSection.Course.Credits;
                        totalCredits += reg.ClassSection.Course.Credits;
                    }
                }
                var termGpa = totalCredits > 0 ? gpa / totalCredits : 0;
                if (termGpa >= 3.5m)
                {
                    awarded.Add(new ScholarshipRecipientResponse
                    {
                        StudentId = student.Id,
                        StudentName = student.User.FullName,
                        Mssv = student.User.UserCode,
                        AwardDate = DateTime.UtcNow,
                        Amount = 1000000 // giả lập
                    });
                }
                else
                {
                    notAwarded.Add(new ScholarshipRecipientResponse
                    {
                        StudentId = student.Id,
                        StudentName = student.User.FullName,
                        Mssv = student.User.UserCode,
                        AwardDate = DateTime.UtcNow,
                        Amount = 0
                    });
                }
            }
            return new ScholarshipReviewResponse
            {
                TermId = termId,
                TermName = "Term Name", // cần lấy tên kỳ học thực tế
                AwardedStudents = awarded,
                NotAwardedStudents = notAwarded
            };
        }

        private async Task<ScholarshipResponse> MapToResponse(Guid id)
        {
            var s = await _unitOfWork.GetRepository<Scholarship>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id == id,
                    include: q => q.Include(x => x.Department)
                );
            return new ScholarshipResponse
            {
                Id = s.Id,
                ScholarshipName = s.ScholarshipName,
                Description = s.Description,
                Amount = s.Amount,
                EligibilityCriteria = s.EligibilityCriteria,
				DepartmentId = s.DepartmentId.HasValue ? s.DepartmentId.Value : Guid.Empty,
				DepartmentName = s.Department?.DepartmentName
            };
        }
    }
} 