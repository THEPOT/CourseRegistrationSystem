namespace CDQTSystem_API.Payload.Response
{
	public class EvaluationQuestionResponse
	{
		public Guid Id { get; set; }
		public string QuestionText { get; set; }
		public string QuestionTextLocalized { get; set; }
		public string QuestionType { get; set; }
		public string Category { get; set; }
		public int OrderIndex { get; set; }
		public bool IsActive { get; set; }
		public List<EvaluationOptionResponse> Options { get; set; }
	}
	public class EvaluationOptionResponse
	{
		public Guid Id { get; set; }
		public string OptionText { get; set; }
	}
}
