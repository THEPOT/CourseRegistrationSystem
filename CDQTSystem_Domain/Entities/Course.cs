using System;
using System.Collections.Generic;

namespace CDQTSystem_Domain.Entities;

public partial class Course
{
    public Guid Id { get; set; }

    public string CourseCode { get; set; } = null!;

    public string CourseName { get; set; } = null!;

    public int Credits { get; set; }

    public string? Description { get; set; }

    public string? LearningOutcomes { get; set; }

    public Guid DepartmentId { get; set; }

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();

    public virtual ICollection<CourseSyllabus> CourseSyllabi { get; set; } = new List<CourseSyllabus>();

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Course> CorequisiteCourses { get; set; } = new List<Course>();

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    public virtual ICollection<Course> CoursesNavigation { get; set; } = new List<Course>();

    public virtual ICollection<Major> Majors { get; set; } = new List<Major>();

    public virtual ICollection<Course> PrerequisiteCourses { get; set; } = new List<Course>();
}
