namespace CDQTSystem_API.Payload.Response
{
public class StudentDetailedInfoResponse
{
    // Tab 1: Thông tin cá nhân
    public string Mssv { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; }
    public DateOnly EnrollmentDate { get; set; }
    public string Status { get; set; }
    public string ImageUrl { get; set; }

    // Tab 2: Học bổng
    public CurrentScholarship CurrentScholarship { get; set; }
    public List<ScholarshipHistory> ScholarshipHistory { get; set; }

    // Tab 3: Ngành học
    public MajorInfo MajorInfo { get; set; }

    // Tab 4: Học phí
    public CurrentTuition? CurrentTuition { get; set; }
    public List<TuitionHistory> TuitionHistory { get; set; } = new();
}

public class CurrentScholarship
{
    public string ScholarshipName { get; set; }
    public decimal Amount { get; set; }
    public DateOnly StartDate { get; set; }
}

public class ScholarshipHistory
{
    public string ScholarshipName { get; set; }
    public decimal Amount { get; set; }
    public DateOnly StartDate { get; set; }
}

public class MajorInfo
{
    public Guid MajorId { get; set; }
    public string MajorCode { get; set; }
    public int RequiredCredits { get; set; }
    public int CompletedCredits { get; set; }
    public decimal CompletionPercentage { get; set; }
    public List<CourseStatus> Courses { get; set; }
}

public class CourseStatus
{
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public string Status { get; set; }
    public decimal? Grade { get; set; }
}

public class CurrentTuition
{
    public string TuitionName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
public class TuitionHistory
{
    public string TuitionName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly DueDate { get; set; }
    public string Status { get; set; } = string.Empty;
}
}