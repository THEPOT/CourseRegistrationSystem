using System;
using System.Collections.Generic;

namespace CourseRegistration_Domain.Entities;

public partial class Course
{
    public Guid Id { get; set; }

    public string CourseCode { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public int Credits { get; set; }

    public string? Description { get; set; }

    public string? LearningOutcomes { get; set; }

    public Guid FacultyId { get; set; }

    public virtual ICollection<CourseOffering> CourseOfferings { get; set; } = new List<CourseOffering>();

    public virtual ICollection<CourseSyllabus> CourseSyllabi { get; set; } = new List<CourseSyllabus>();

    public virtual Faculty Faculty { get; set; } = null!;

    public virtual ICollection<Course> CorequisiteCourses { get; set; } = new List<Course>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Course> CoursesNavigation { get; set; } = new List<Course>();

    public virtual ICollection<Course> PrerequisiteCourses { get; set; } = new List<Course>();

    public virtual ICollection<Program> Programs { get; set; } = new List<Program>();
}
