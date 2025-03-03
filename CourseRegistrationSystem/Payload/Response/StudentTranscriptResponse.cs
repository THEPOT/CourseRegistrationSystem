// Update StudentTranscriptResponse.cs
namespace CourseRegistration_API.Payload.Response
{

    public class StudentTranscriptSummary
    {
        public Guid StudentId { get; set; }
        public string Mssv { get; set; }
        public string StudentName { get; set; }
        public string ProgramName { get; set; }
        public decimal GPA { get; set; }
        public int TotalCredits { get; set; }
    }
    public class StudentTranscriptResponse
    {
        public string Mssv { get; set; }
        public string StudentName { get; set; }
        public string ProgramName { get; set; }
        public List<TermGrades> Terms { get; set; } = new List<TermGrades>();
        public decimal CumulativeGPA { get; set; }
        public int TotalCredits { get; set; }
        public int TotalCreditsPassed { get; set; }
        // The original CourseGrades list will be kept for backward compatibility
        public List<CourseGrade> CourseGrades { get; set; } = new List<CourseGrade>();
    }

    public class TermGrades
    {
        public string TermName { get; set; }
        public List<CourseGrade> Courses { get; set; } = new List<CourseGrade>();
        public decimal TermGPA { get; set; }
        public int TermCredits { get; set; }
    }

    public class CourseGrade
    {
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public decimal? Grade { get; set; } // Numeric grade (quality points)
        public string GradeValue { get; set; } // Letter grade (A, B, C, etc.)
        public string Semester { get; set; }
        public bool IsPassed { get; set; } // Added to indicate passing status
    }
}