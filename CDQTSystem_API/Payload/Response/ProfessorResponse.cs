namespace CDQTSystem_API.Payload.Response
{
    public class ProfessorResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string DepartmentName { get; set; }
        public Guid DepartmentId { get; set; }
        public string ImageUrl { get; set; }
    }
}