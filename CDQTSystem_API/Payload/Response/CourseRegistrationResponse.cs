using System;

namespace CDQTSystem_API.Payload.Response
{
    public class CourseRegistrationResponse
    {
        public Guid RegistrationId { get; set; }
        public Guid CourseOfferingId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int Credits { get; set; }
        public Guid ProfessorId { get; set; }
        public string ProfessorName { get; set; }
        public string Classroom { get; set; }
        public string Schedule { get; set; }
        public string Status { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string TuitionStatus { get; set; }
    }
}