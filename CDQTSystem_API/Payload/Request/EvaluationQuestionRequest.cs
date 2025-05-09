namespace CDQTSystem_API.Payload.Request
{
	public class EvaluationQuestionRequest
	{
		public string QuestionText { get; set; }
		public string QuestionTextLocalized { get; set; }
		public string QuestionType { get; set; }
		public string Category { get; set; }
		public int OrderIndex { get; set; }
		public bool IsActive { get; set; }
		public List<EvaluationOptionRequest> Options { get; set; }
	}
	public class EvaluationOptionRequest
	{
		public string OptionText { get; set; }
	}
}
