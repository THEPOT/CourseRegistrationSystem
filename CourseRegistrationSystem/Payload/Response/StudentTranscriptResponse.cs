public class StudentTranscriptResponse
{
    public string Mssv { get; set; }
    public string StudentName { get; set; }
    public string ProgramName { get; set; }
    public List<CourseGrade> CourseGrades { get; set; } = new List<CourseGrade>();
    public decimal GPA { get; set; }
    public int TotalCredits { get; set; }
}

public class CourseGrade
{
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public decimal? Grade { get; set; }
    public string Semester { get; set; }
} 