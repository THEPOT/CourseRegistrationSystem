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
	public class TuitionService : ITuitionService
	{
		private readonly IUnitOfWork<DbContext> _unitOfWork;
		public TuitionService(IUnitOfWork<DbContext> unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<TuitionRateResponse>> GetTuitionRates()
		{
			var rates = await _unitOfWork.GetRepository<TuitionPolicy>()
				.GetListAsync(predicate: p => p.PolicyName == "Base" || p.PolicyName == "Credit");
			return rates.Select(r => new TuitionRateResponse
			{
				RateName = r.PolicyName,
				Amount = r.Amount
			}).ToList();
		}

		public async Task<bool> UpdateTuitionRates(List<TuitionRateRequest> rates)
		{
			var repo = _unitOfWork.GetRepository<TuitionPolicy>();
			foreach (var rate in rates)
			{
				var policy = await repo.SingleOrDefaultAsync(predicate: p => p.PolicyName == rate.RateName && (p.PolicyName == "Base" || p.PolicyName == "Credit"));
				if (policy != null)
				{
					policy.Amount = rate.Amount;
					repo.UpdateAsync(policy);
				}
			}
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<List<ProgramRateResponse>> GetProgramRates()
		{
			var rates = await _unitOfWork.GetRepository<TuitionPolicy>()
				.GetListAsync(predicate: p => p.PolicyName == "Program");
			return rates.Select(r => new ProgramRateResponse
			{
				ProgramId = r.MajorId ?? Guid.Empty,
				ProgramName = r.PolicyName,
				Amount = r.Amount
			}).ToList();
		}

		public async Task<bool> UpdateProgramRates(List<ProgramRateRequest> rates)
		{
			var repo = _unitOfWork.GetRepository<TuitionPolicy>();
			foreach (var rate in rates)
			{
				var policy = await repo.SingleOrDefaultAsync(predicate: p => p.MajorId == rate.ProgramId && p.PolicyName == "Program");
				if (policy != null)
				{
					policy.Amount = rate.Amount;
					repo.UpdateAsync(policy);
				}
			}
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<List<TuitionPaymentResponse>> GetTuitionPayments(TuitionPaymentFilterRequest filter)
		{
			var payments = await _unitOfWork.GetRepository<StudentTuition>()
				.GetListAsync(
					predicate: p => (!filter.StudentId.HasValue || p.StudentId == filter.StudentId)
						&& (!filter.TermId.HasValue || p.SemesterId == filter.TermId)
						&& (string.IsNullOrEmpty(filter.Status) || p.PaymentStatus == filter.Status),
					include: q => q.Include(p => p.Student).ThenInclude(s => s.User)
						.Include(p => p.Semester)
				);
			return payments.Skip((filter.Page - 1) * filter.Size).Take(filter.Size)
				.Select(p => new TuitionPaymentResponse
				{
					PaymentId = p.Id,
					StudentId = p.StudentId,
					StudentName = p.Student.User.FullName,
					Mssv = p.Student.User.UserCode,
					TermId = p.SemesterId,
					TermName = p.Semester.SemesterName,
					Amount = p.TotalAmount,
					Status = p.PaymentStatus,
					PaymentDate = p.PaymentDate ?? DateTime.MinValue
				}).ToList();
		}

		public async Task<TuitionStatisticsResponse> GetTuitionStatistics()
		{
			var tuitions = await _unitOfWork.GetRepository<StudentTuition>()
				.GetListAsync(include: q => q.Include(t => t.Student).ThenInclude(s => s.Major));
			var totalStudents = tuitions.Select(t => t.StudentId).Distinct().Count();
			var paid = tuitions.Where(predicate: t => t.PaymentStatus == "Paid");
			var unpaid = tuitions.Where(predicate: t => t.PaymentStatus != "Paid");
			var stats = new TuitionStatisticsResponse
			{
				TotalStudents = totalStudents,
				PaidStudents = paid.Select(t => t.StudentId).Distinct().Count(),
				UnpaidStudents = unpaid.Select(t => t.StudentId).Distinct().Count(),
				TotalAmount = tuitions.Sum(t => t.TotalAmount),
				PaidAmount = paid.Sum(t => t.TotalAmount),
				UnpaidAmount = unpaid.Sum(t => t.TotalAmount),
				ProgramStatistics = tuitions.GroupBy(t => t.Student.Major.MajorName)
					.Select(g => new ProgramTuitionStatistic
					{
						ProgramName = g.Key,
						StudentCount = g.Select(t => t.StudentId).Distinct().Count(),
						TotalAmount = g.Sum(t => t.TotalAmount),
						PaidAmount = g.Where(predicate: t => t.PaymentStatus == "Paid").Sum(t => t.TotalAmount),
						UnpaidAmount = g.Where(predicate: t => t.PaymentStatus != "Paid").Sum(t => t.TotalAmount)
					}).ToList()
			};
			return stats;
		}

		public async Task<TuitionPeriodResponse> CreateTuitionPeriod(TuitionPeriodCreateRequest request)
		{
			var period = new TuitionPeriod
			{
				Id = Guid.NewGuid(),
				SemesterId = request.SemesterId,
				StartDate = request.StartDate,
				EndDate = request.EndDate,
				Status = "ACTIVE"
			};
			await _unitOfWork.GetRepository<TuitionPeriod>().InsertAsync(period);
			await _unitOfWork.CommitAsync();

			// Lấy thông tin semester và policies nếu có
			var semester = await _unitOfWork.GetRepository<Semester>().SingleOrDefaultAsync(predicate: s => s.Id == period.SemesterId);
			var policies = await _unitOfWork.GetRepository<TuitionPolicy>().GetListAsync();


			return new TuitionPeriodResponse
			{
				Id = period.Id,
				SemesterId = period.SemesterId,
				SemesterName = semester?.SemesterName ?? string.Empty,
				StartDate = period.StartDate,
				EndDate = period.EndDate,
				Status = period.Status,
				BaseTuitionFee = 0,
				CreditFee = 0,
				PaymentInstructions = string.Empty,
				Policies = policies.Select(p => new TuitionPolicyInfo
				{
					Id = p.Id,
					PolicyType = "",
					Amount = p.Amount,
					DiscountPercentage = 0,
					Description = p.Description,
					StudentsApplied = 0
				}).ToList(),
			};
		}

		public async Task<bool> UpdateTuitionPeriodStatus(Guid periodId, string status)
		{
			var repo = _unitOfWork.GetRepository<TuitionPeriod>();
			var period = await repo.SingleOrDefaultAsync(predicate: p => p.Id == periodId);
			if (period == null) return false;
			period.Status = status;
			repo.UpdateAsync(period);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<bool> RecordTuitionPayment(TuitionPaymentRequest request)
		{
			var repo = _unitOfWork.GetRepository<StudentTuition>();
			var tuition = await repo.SingleOrDefaultAsync(predicate: t => t.StudentId == request.StudentId && t.SemesterId == request.SemesterId);
			if (tuition == null) return false;
			tuition.PaymentStatus = "Paid";
			tuition.PaymentDate = DateTime.UtcNow;
			tuition.AmountPaid = request.AmountPaid;
			repo.UpdateAsync(tuition);
			await _unitOfWork.CommitAsync();
			return true;
		}

		public async Task<TuitionStatusResponse> GetStudentTuitionStatus(Guid studentId, Guid semesterId)
		{
			var tuition = await _unitOfWork.GetRepository<StudentTuition>()
				.SingleOrDefaultAsync(predicate: t => t.StudentId == studentId && t.SemesterId == semesterId);

			if (tuition == null) return null;

			return new TuitionStatusResponse
			{
				TuitionId = tuition.Id,
				StudentId = tuition.StudentId,
				SemesterId = tuition.SemesterId,
				TotalAmount = tuition.TotalAmount,
				AmountPaid = tuition.AmountPaid,
				PaymentStatus = tuition.PaymentStatus,
				DueDate = tuition.DueDate
			};
		}

		public async Task<List<TuitionStudentSummary>> GetTuitionSummary(Guid termId)
		{
			var tuitions = await _unitOfWork.GetRepository<StudentTuition>()
				.GetListAsync(predicate: t => t.SemesterId == termId,
					include: q => q.Include(t => t.Student).ThenInclude(s => s.User));

			return tuitions.Select(t => new TuitionStudentSummary
			{
				StudentId = t.StudentId,
				StudentName = t.Student.User.FullName,
				Mssv = t.Student.User.UserCode,
				TotalAmount = t.TotalAmount,
				AmountPaid = t.AmountPaid,
				PaymentStatus = t.PaymentStatus
			}).ToList();
		}

		public async Task<TuitionPeriodResponse> GetCurrentTuitionPeriod()
		{
			var now = DateTime.UtcNow;
			var period = await _unitOfWork.GetRepository<TuitionPeriod>()
				.SingleOrDefaultAsync(predicate: p => p.StartDate <= now && p.EndDate >= now && p.Status == "ACTIVE");
			if (period == null) return null;
			var semester = await _unitOfWork.GetRepository<Semester>().SingleOrDefaultAsync(predicate: s => s.Id == period.SemesterId);
			var policies = await _unitOfWork.GetRepository<TuitionPolicy>().GetListAsync();
			return new TuitionPeriodResponse
			{
				Id = period.Id,
				SemesterId = period.SemesterId,
				SemesterName = semester?.SemesterName ?? string.Empty,
				StartDate = period.StartDate,
				EndDate = period.EndDate,
				Status = period.Status,
				BaseTuitionFee = 0,
				CreditFee = 0,
				PaymentInstructions = string.Empty,
				Policies = policies.Select(p => new TuitionPolicyInfo
				{
					Id = p.Id,
					PolicyType = "",
					Amount = p.Amount,
					DiscountPercentage = 0,
					Description = p.Description,
					StudentsApplied = 0
				}).ToList(),
				Statistics = null
			};
		}
	}
}