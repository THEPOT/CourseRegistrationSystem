﻿using AutoMapper;
using CourseRegistration_API.Payload.Request;
using CourseRegistration_API.Payload.Response;
using CourseRegistration_API.Services.Interface;
using CourseRegistration_Domain.Entities;
using CourseRegistration_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseRegistration_API.Services.Implements
{
	public class CourseService : BaseService<CourseService>, ICourseService
	{
		public CourseService(IUnitOfWork<DbContext> unitOfWork, ILogger<CourseService> logger,
						  IMapper mapper, IHttpContextAccessor httpContextAccessor)
			: base(unitOfWork, logger, mapper, httpContextAccessor)
		{
		}

		public async Task<List<CourseBasicInfo>> GetAllCourses()
		{
			var courses = await _unitOfWork.GetRepository<Course>()
				.GetListAsync();

			return courses
				.OrderBy(c => c.CourseCode)
				.Select(c => new CourseBasicInfo
				{
					Id = c.Id,
					CourseCode = c.CourseCode,
					CourseName = c.CourseName
				}).ToList();
		}

		public async Task<CourseDetailsResponse> GetCourseByCode(string courseCode)
		{
			var course = await _unitOfWork.GetRepository<Course>()
				.SingleOrDefaultAsync(
					predicate: c => c.CourseCode == courseCode,
					include: q => q
						.Include(c => c.Department)
						.Include(c => c.PrerequisiteCourses)
						.Include(c => c.CorequisiteCourses)
				);

			if (course == null)
				return null;

			return MapCourseToResponse(course);
		}

		public async Task<CourseDetailsResponse> GetCourseById(Guid courseId)
		{
			var course = await _unitOfWork.GetRepository<Course>()
				.SingleOrDefaultAsync(
					predicate: c => c.Id == courseId,
					include: q => q
						.Include(c => c.Department) // Changed from Faculty to Department
						.Include(c => c.PrerequisiteCourses)
						.Include(c => c.CorequisiteCourses)
				);

			if (course == null)
				return null;

			return MapCourseToResponse(course);
		}

		public async Task<List<CourseDetailsResponse>> SearchCourses(string keyword)
		{
			if (string.IsNullOrWhiteSpace(keyword))
				return new List<CourseDetailsResponse>();

			keyword = keyword.ToLower();

			var courses = await _unitOfWork.GetRepository<Course>()
				.GetListAsync(
					predicate: c => c.CourseCode.ToLower().Contains(keyword) ||
								c.CourseName.ToLower().Contains(keyword) ||
								c.Description.ToLower().Contains(keyword),
					include: q => q
						.Include(c => c.Department) // Changed from Faculty to Department
						.Include(c => c.PrerequisiteCourses)
						.Include(c => c.CorequisiteCourses)
				);

			return courses
				.OrderBy(c => c.CourseCode)
				.Select(c => MapCourseToResponse(c))
				.ToList();
		}

		public async Task<CourseDetailsResponse> CreateCourse(CourseCreateRequest request)
		{
			try
			{

				// Check if course code already exists
				var existingCourse = await _unitOfWork.GetRepository<Course>()
					.SingleOrDefaultAsync(predicate: c => c.CourseCode == request.CourseCode);

				if (existingCourse != null)
					throw new BadHttpRequestException($"Course with code '{request.CourseCode}' already exists");

				// Check if faculty exists
				var faculty = await _unitOfWork.GetRepository<Department>()
					.SingleOrDefaultAsync(predicate: f => f.Id == request.DepartmentId);

				if (faculty == null)
					throw new BadHttpRequestException("Faculty not found");

				// Create new course
				var course = new Course
				{
					Id = Guid.NewGuid(),
					CourseCode = request.CourseCode,
					CourseName = request.CourseName,
					Credits = request.Credits,
					Description = request.Description,
					LearningOutcomes = request.LearningOutcomes,
					DepartmentId = request.DepartmentId
				};

				// Insert the course first to get an ID
				await _unitOfWork.GetRepository<Course>().InsertAsync(course);
				await _unitOfWork.CommitAsync();

				// Handle prerequisites and corequisites
				if (request.PrerequisiteCourseIds?.Any() == true)
				{
					var prerequisites = await _unitOfWork.GetRepository<Course>()
						.GetListAsync(predicate: c => request.PrerequisiteCourseIds.Contains(c.Id));

					foreach (var prerequisite in prerequisites)
					{
						course.PrerequisiteCourses.Add(prerequisite);
					}
				}

				if (request.CorequisiteCourseIds?.Any() == true)
				{
					var corequisites = await _unitOfWork.GetRepository<Course>()
						.GetListAsync(predicate: c => request.CorequisiteCourseIds.Contains(c.Id));

					foreach (var corequisite in corequisites)
					{
						course.CorequisiteCourses.Add(corequisite);
					}
				}

				await _unitOfWork.CommitAsync();

				// Reload the course with all relationships
				return await GetCourseById(course.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating course: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<CourseDetailsResponse> UpdateCourse(Guid courseId, CourseUpdateRequest request)
		{
			try
			{

				// Get the course with its relationships
				var course = await _unitOfWork.GetRepository<Course>()
					.SingleOrDefaultAsync(
						predicate: c => c.Id == courseId,
						include: q => q
							.Include(c => c.PrerequisiteCourses)
							.Include(c => c.CorequisiteCourses)
					);

				if (course == null)
					throw new BadHttpRequestException("Course not found");

				// Check if faculty exists
				var faculty = await _unitOfWork.GetRepository<Department>()
					.SingleOrDefaultAsync(predicate: f => f.Id == request.DepartmentId);

				if (faculty == null)
					throw new BadHttpRequestException("Faculty not found");

				// Update basic properties
				course.CourseName = request.CourseName;
				course.Credits = request.Credits;
				course.Description = request.Description;
				course.LearningOutcomes = request.LearningOutcomes;
				course.DepartmentId = request.DepartmentId;


				// Clear and re-add prerequisites
				course.PrerequisiteCourses.Clear();
				if (request.PrerequisiteCourseIds?.Any() == true)
				{
					var prerequisites = await _unitOfWork.GetRepository<Course>()
						.GetListAsync(predicate: c => request.PrerequisiteCourseIds.Contains(c.Id));

					foreach (var prerequisite in prerequisites)
					{
						course.PrerequisiteCourses.Add(prerequisite);
					}
				}

				// Clear and re-add corequisites
				course.CorequisiteCourses.Clear();
				if (request.CorequisiteCourseIds?.Any() == true)
				{
					var corequisites = await _unitOfWork.GetRepository<Course>()
						.GetListAsync(predicate: c => request.CorequisiteCourseIds.Contains(c.Id));

					foreach (var corequisite in corequisites)
					{
						course.CorequisiteCourses.Add(corequisite);
					}
				}

				await _unitOfWork.CommitAsync();

				// Reload the course with all relationships
				return await GetCourseById(course.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating course: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<bool> DeleteCourse(Guid courseId)
		{
			try
			{

				// Check if course is used in any course offerings
				var offerings = await _unitOfWork.GetRepository<ClassSection>()
					.GetListAsync(predicate: co => co.CourseId == courseId);
				var hasOfferings = offerings.Any();

				if (hasOfferings)
					throw new BadHttpRequestException("Cannot delete course with existing offerings");
				// Check if the course is a prerequisite for other courses
				var coursesWithPrereqs = await _unitOfWork.GetRepository<Course>()
					.GetListAsync(predicate: c => c.PrerequisiteCourses.Any(pc => pc.Id == courseId));
				var isPrerequisite = coursesWithPrereqs.Any();

				if (isPrerequisite)
					throw new BadHttpRequestException("Cannot delete course that is a prerequisite for other courses");

				// Get the course with its relationships
				var course = await _unitOfWork.GetRepository<Course>()
					.SingleOrDefaultAsync(
						predicate: c => c.Id == courseId,
						include: q => q
							.Include(c => c.PrerequisiteCourses)
							.Include(c => c.CorequisiteCourses)
							.Include(c => c.CourseSyllabi)
					);

				if (course == null)
					return false;

				// Clear relationships
				course.PrerequisiteCourses.Clear();
				course.CorequisiteCourses.Clear();

				// Delete syllabi
				foreach (var syllabus in course.CourseSyllabi.ToList())
				{
					_unitOfWork.GetRepository<CourseSyllabus>().DeleteAsync(syllabus);
				}

				// Delete the course
				_unitOfWork.GetRepository<Course>().DeleteAsync(course);
				await _unitOfWork.CommitAsync();

				return true;
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error deleting course: {Message}", ex.Message);
				throw;
			}
		}

		public async Task<CourseSyllabusResponse> GetLatestSyllabus(Guid courseId)
		{
			var course = await _unitOfWork.GetRepository<Course>()
				.SingleOrDefaultAsync(
					predicate: c => c.Id == courseId,
					include: q => q.Include(c => c.CourseSyllabi)
				);

			if (course == null)
				throw new BadHttpRequestException("Course not found");

			var latestSyllabus = course.CourseSyllabi
				.OrderByDescending(s => s.CreatedDate)
				.FirstOrDefault();

			if (latestSyllabus == null)
				return null;

			return new CourseSyllabusResponse
			{
				Id = latestSyllabus.Id,
				CourseId = course.Id,
				CourseCode = course.CourseCode,
				CourseName = course.CourseName,
				SyllabusContent = latestSyllabus.SyllabusContent,
				Version = latestSyllabus.Version,
				CreatedDate = latestSyllabus.CreatedDate,
				UpdatedDate = latestSyllabus.UpdatedDate
			};
		}

		public async Task<List<CourseSyllabusResponse>> GetAllSyllabusVersions(Guid courseId)
		{
			var course = await _unitOfWork.GetRepository<Course>()
				.SingleOrDefaultAsync(
					predicate: c => c.Id == courseId,
					include: q => q.Include(c => c.CourseSyllabi)
				);

			if (course == null)
				throw new BadHttpRequestException("Course not found");

			return course.CourseSyllabi
				.OrderByDescending(s => s.CreatedDate)
				.Select(s => new CourseSyllabusResponse
				{
					Id = s.Id,
					CourseId = course.Id,
					CourseCode = course.CourseCode,
					CourseName = course.CourseName,
					SyllabusContent = s.SyllabusContent,
					Version = s.Version,
					CreatedDate = s.CreatedDate,
					UpdatedDate = s.UpdatedDate
				})
				.ToList();
		}

		public async Task<CourseSyllabusResponse> CreateSyllabus(Guid courseId, SyllabusCreateRequest request)
		{
			try
			{


				// Check if course exists
				var course = await _unitOfWork.GetRepository<Course>()
					.SingleOrDefaultAsync(predicate: c => c.Id == courseId);

				if (course == null)
					throw new BadHttpRequestException("Course not found");

				var syllabus = new CourseSyllabus
				{
					Id = Guid.NewGuid(),
					CourseId = courseId,
					SyllabusContent = request.SyllabusContent,
					Version = request.Version,
					CreatedDate = DateTime.Now
				};

				await _unitOfWork.GetRepository<CourseSyllabus>().InsertAsync(syllabus);
				await _unitOfWork.CommitAsync();


				return new CourseSyllabusResponse
				{
					Id = syllabus.Id,
					CourseId = course.Id,
					CourseCode = course.CourseCode,
					CourseName = course.CourseName,
					SyllabusContent = syllabus.SyllabusContent,
					Version = syllabus.Version,
					CreatedDate = syllabus.CreatedDate,
					UpdatedDate = syllabus.UpdatedDate
				};
			}
			catch (Exception ex)
			{

				_logger.LogError(ex, "Error creating syllabus: {Message}", ex.Message);
				throw;
			}
		}

		private CourseDetailsResponse MapCourseToResponse(Course course)
		{
			return new CourseDetailsResponse
			{
				Id = course.Id,
				CourseCode = course.CourseCode,
				CourseName = course.CourseName,
				Credits = course.Credits,
				Description = course.Description,
				LearningOutcomes = course.LearningOutcomes,
				DepartmentId = course.DepartmentId, 
				DepartmentName = course.Department?.DepartmentName,
				Prerequisites = course.PrerequisiteCourses?.Select(c => new CourseBasicInfo
				{
					Id = c.Id,
					CourseCode = c.CourseCode,
					CourseName = c.CourseName
				}).ToList() ?? new List<CourseBasicInfo>(),
				Corequisites = course.CorequisiteCourses?.Select(c => new CourseBasicInfo
				{
					Id = c.Id,
					CourseCode = c.CourseCode,
					CourseName = c.CourseName
				}).ToList() ?? new List<CourseBasicInfo>()
			};
		}
	}
}