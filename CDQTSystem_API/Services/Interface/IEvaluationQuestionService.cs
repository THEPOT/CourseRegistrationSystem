using CDQTSystem_API.Payload.Request;
using CDQTSystem_API.Payload.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Interface
{
	public interface IEvaluationQuestionService
	{
		Task<List<EvaluationQuestionResponse>> GetAllQuestions();
		Task<EvaluationQuestionResponse> GetQuestionById(Guid id);
		Task<EvaluationQuestionResponse> CreateQuestion(EvaluationQuestionRequest request);
		Task<EvaluationQuestionResponse> UpdateQuestion(Guid id, EvaluationQuestionRequest request);
		Task<bool> DeleteQuestion(Guid id);
	}
}
