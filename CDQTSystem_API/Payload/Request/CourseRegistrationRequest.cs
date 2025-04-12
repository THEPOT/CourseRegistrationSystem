public class CourseRegistrationRequest
{
    public Guid StudentId { get; set; }
    public Guid CourseOfferingId { get; set; }
}

public class AvailableCourseResponse
{
    public Guid CourseOfferingId { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
    public int Credits { get; set; }
    public string ProfessorName { get; set; }
    public string Schedule { get; set; }
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public int AvailableSlots { get; set; }
    public bool PrerequisitesSatisfied { get; set; }
    public List<string> Prerequisites { get; set; }
}
