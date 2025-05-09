namespace CDQTSystem_API.Payload.Request
{
    public class GradeEntryRequest
{
    public Guid CourseRegistrationId { get; set; }
    public decimal QualityPoints { get; set; } // Điểm số tích lũy
    public string GradeValue { get; set; }     // Điểm chữ (A, B, C, D, F)
}
}