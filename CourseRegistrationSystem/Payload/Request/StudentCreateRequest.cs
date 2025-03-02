public class StudentCreateRequest
{
    public string Email { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public Guid ProgramId { get; set; }
    public DateOnly? AdmissionDate { get; set; }
    public string AdmissionStatus { get; set; }
    public string ImageUrl { get; set; }
} 