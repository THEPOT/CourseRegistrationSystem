namespace CDQTSystem_API.Payload.Response
{
	public class StudentProgressResponse
	{
		public StudentInfoDto Student { get; set; }
		public List<CategoryDto> Categories { get; set; }
	}

	public class StudentInfoDto
	{
		public string Name { get; set; }
		public Guid Id { get; set; }
		public string Program { get; set; }
		public int AdmissionYear { get; set; }
		public int ExpectedGraduation { get; set; }
		public int TotalCredits { get; set; }
		public int RequiredCredits { get; set; }
		public double Gpa { get; set; }
	}

	public class CategoryDto
	{
		public string Name { get; set; }
		public int Completed { get; set; }
		public int Required { get; set; }
		public List<CourseDto> Courses { get; set; }
	}

	public class CourseDto
	{
		public string Code { get; set; }
		public string Name { get; set; }
		public int Credits { get; set; }
		public string Status { get; set; }
		public string Grade { get; set; }
	}
}