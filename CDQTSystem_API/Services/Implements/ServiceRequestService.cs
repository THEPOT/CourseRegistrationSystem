// Services/Implements/ServiceRequestService.cs
using AutoMapper;
using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Domain.Paginate;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Implements
{
	public class ServiceRequestService : BaseService<ServiceRequestService>, IServiceRequestService
	{
		public ServiceRequestService(IUnitOfWork<DbContext> unitOfWork, ILogger<ServiceRequestService> logger,
						  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<IPaginate<ServiceRequestResponse>> GetAllServiceRequests(string status = null, int page = 1, int size = 10, string search = null)
		{

			var requests = await _unitOfWork.GetRepository<ServiceRequest>().GetPagingListAsync(
				selector: sr => new ServiceRequestResponse
				{
					Id = sr.Id,
					StudentId = sr.StudentId,
					StudentName = sr.Student.User.FullName,
					Mssv = sr.Student.User.UserCode,
					ServiceType = sr.ServiceType,
					RequestDate = sr.RequestDate,
					Status = sr.Status,
					Details = sr.Details,
					StaffComments = sr.Details // Same as above
				},
				predicate: sr => (string.IsNullOrEmpty(status) || sr.Status == status)
					&& (string.IsNullOrEmpty(search) || sr.Student.User.FullName.Contains(search) || sr.Student.User.UserCode.Contains(search)),
				include: q => q.Include(sr => sr.Student).ThenInclude(s => s.User),
				page: page,
				size: size
			);

			// Map sang response
			return requests;
		}

		public async Task<IPaginate<ServiceRequestResponse>> GetStudentServiceRequests(Guid userId, int page, int size )
		{
			var student = await _unitOfWork.GetRepository<Student>()
				.SingleOrDefaultAsync(predicate: s => s.UserId == userId);
			var responses = await _unitOfWork.GetRepository<ServiceRequest>().GetPagingListAsync(
				selector: sr => new ServiceRequestResponse
				{
					Id = sr.Id,
					StudentId = sr.StudentId,
					StudentName = sr.Student.User.FullName,
					Mssv = sr.Student.User.UserCode,
					ServiceType = sr.ServiceType,
					RequestDate = sr.RequestDate,
					Status = sr.Status,
					Details = sr.Details,
					StaffComments = sr.Details // Same as above
				},
				predicate: sr => sr.StudentId == student.Id,
				include: q => q.Include(sr => sr.Student)
							  .ThenInclude(s => s.User)
			);

			return responses;
		}

		public async Task<ServiceRequestResponse> GetServiceRequestById(Guid id)
		{
			var request = await _unitOfWork.GetRepository<ServiceRequest>()
				.SingleOrDefaultAsync(
					predicate: sr => sr.Id == id,
					include: q => q.Include(sr => sr.Student)
								  .ThenInclude(s => s.User)
				);

			if (request == null)
				return null;

			return new ServiceRequestResponse
			{
				Id = request.Id,
				StudentId = request.StudentId,
				StudentName = request.Student.User.FullName,
				Mssv = request.Student.User.UserCode,
				ServiceType = request.ServiceType,
				RequestDate = request.RequestDate,
				Status = request.Status,
				Details = request.Details,
				StaffComments = request.Details // Same as above
			};
		}

		public async Task<ServiceRequestResponse> CreateServiceRequest(ServiceRequestCreateRequest request)
		{
			try
			{


				// Check if student exists
				var student = await _unitOfWork.GetRepository<Student>()
					.SingleOrDefaultAsync(
						predicate: s => s.Id == request.StudentId,
						include: q => q.Include(s => s.User)
					);

				if (student == null)
					throw new BadHttpRequestException("Student not found");

				// Validate service type
				var validServiceTypes = new[] { "Certificate", "Transcript", "LeaveOfAbsence", "CreditOverload",
					"ProgramChange", "AddDrop", "Withdraw", "Graduation", "AcademicAdvising",
					"ClassroomBorrow", "TemporaryWithdraw", "PermanentWithdraw", "Other" };

				if (!validServiceTypes.Contains(request.ServiceType))
					throw new BadHttpRequestException($"Invalid service type. Valid types: {string.Join(", ", validServiceTypes)}");

				var serviceRequest = new ServiceRequest
				{
					Id = Guid.NewGuid(),
					StudentId = request.StudentId,
					ServiceType = request.ServiceType,
					RequestDate = DateTime.Now,
					Status = "Pending", // Always start as pending
					Details = request.Details
				};

				await _unitOfWork.GetRepository<ServiceRequest>().InsertAsync(serviceRequest);
				await _unitOfWork.CommitAsync();

				return new ServiceRequestResponse
				{
					Id = serviceRequest.Id,
					StudentId = serviceRequest.StudentId,
					StudentName = student.User.FullName,
					Mssv = student.User.UserCode,
					ServiceType = serviceRequest.ServiceType,
					RequestDate = serviceRequest.RequestDate,
					Status = serviceRequest.Status,
					Details = serviceRequest.Details
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating service request: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<ServiceRequestResponse> UpdateServiceRequestStatus(Guid id, ServiceRequestStatusUpdateRequest request)
		{
			try
			{

				var serviceRequest = await _unitOfWork.GetRepository<ServiceRequest>()
					.SingleOrDefaultAsync(
						predicate: sr => sr.Id == id,
						include: q => q.Include(sr => sr.Student)
									  .ThenInclude(s => s.User)
					);

				if (serviceRequest == null)
					return null;

				// Validate status
				var validStatuses = new[] { "Pending", "Approved", "Denied" };
				if (!validStatuses.Contains(request.Status))
					throw new BadHttpRequestException($"Invalid status. Valid statuses: {string.Join(", ", validStatuses)}");

				serviceRequest.Status = request.Status;

				// You might want to add a field for staff comments in your entity
				// For now, let's update the Details field with the comments
				if (!string.IsNullOrEmpty(request.Comments))
				{
					serviceRequest.Details += $"\n\nStaff comments: {request.Comments}";
				}

				await _unitOfWork.CommitAsync();

				return new ServiceRequestResponse
				{
					Id = serviceRequest.Id,
					StudentId = serviceRequest.StudentId,
					StudentName = serviceRequest.Student.User.FullName,
					Mssv = serviceRequest.Student.User.UserCode,
					ServiceType = serviceRequest.ServiceType,
					RequestDate = serviceRequest.RequestDate,
					Status = serviceRequest.Status,
					Details = serviceRequest.Details,
					StaffComments = request.Comments
				};
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating service request: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<bool> DeleteServiceRequest(Guid id)
		{
			try
			{

				var serviceRequest = await _unitOfWork.GetRepository<ServiceRequest>()
					.SingleOrDefaultAsync(predicate: sr => sr.Id == id);

				if (serviceRequest == null)
					return false;

				// Only allow deletion if status is Pending
				if (serviceRequest.Status != "Pending")
					throw new BadHttpRequestException("Only pending requests can be deleted");

				_unitOfWork.GetRepository<ServiceRequest>().DeleteAsync(serviceRequest);
				await _unitOfWork.CommitAsync();

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting service request: {Message}", ex.Message);
				throw;
			}
		}
		public async Task<ServiceRequestStatisticsResponse> GetStatistics()
		{
			var repo = _unitOfWork.GetRepository<ServiceRequest>();
			var all = await repo.GetListAsync();
			var byType = all.GroupBy(r => r.ServiceType)
				.Select(g => new ServiceRequestTypeStat { Type = g.Key, Count = g.Count() }).ToList();
			var byStatus = all.GroupBy(r => r.Status)
				.Select(g => new ServiceRequestStatusStat { Status = g.Key, Count = g.Count() }).ToList();
			return new ServiceRequestStatisticsResponse
			{
				Total = all.Count,
				Processed = all.Count(r => r.Status == "Processed"),
				Pending = all.Count(r => r.Status == "Pending"),
				Rejected = all.Count(r => r.Status == "Rejected"),
				ByType = byType,
				ByStatus = byStatus
			};
		}
	}
}