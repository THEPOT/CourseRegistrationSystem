namespace CDQTSystem_API.Payload.Response
{
	public class StudentDetailedTranscriptResponse
	{
		public string Mssv { get; set; }
		public string StudentName { get; set; }
		public string MajorName { get; set; }
		public decimal CumulativeGPA { get; set; }
		public int TotalCredits { get; set; }
		public int TotalCreditsPassed { get; set; }
		public List<SemesterTranscript> Semesters { get; set; } = new List<SemesterTranscript>();
	}

	public class SemesterTranscript
	{
		public string SemesterName { get; set; }
		public decimal SemesterGPA { get; set; }
		public int SemesterCredits { get; set; }
		public List<CourseTranscriptDetail> Courses { get; set; } = new List<CourseTranscriptDetail>();
	}

	public class CourseTranscriptDetail
	{
		public string CourseCode { get; set; }
		public string CourseName { get; set; }
		public int Credits { get; set; }
		public decimal? MidtermScore { get; set; }  // Điểm GK
		public decimal? FinalScore { get; set; }    // Điểm CK
		public string LetterGrade { get; set; }     // Điểm chữ
		public string Result { get; set; }          // Kết quả (Đạt/Không đạt)
	}
}
