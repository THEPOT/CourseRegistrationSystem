using CDQTSystem_API.Payload.Request;

using CDQTSystem_API.Payload.Response;
using CDQTSystem_API.Services.Interface;
using CDQTSystem_Domain.Entities;
using CDQTSystem_Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CDQTSystem_API.Services.Implements
{
    public class EvaluationQuestionService : IEvaluationQuestionService
    {
        private readonly IUnitOfWork<DbContext> _unitOfWork;
        public EvaluationQuestionService(IUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EvaluationQuestionResponse>> GetAllQuestions()
        {
            var questions = await _unitOfWork.GetRepository<EvaluationQuestion>()
                .GetListAsync(include: q => q.Include(x => x.EvaluationOptions));
            return questions.Select(q => MapToResponse(q)).ToList();
        }

        public async Task<EvaluationQuestionResponse> GetQuestionById(Guid id)
        {
            var question = await _unitOfWork.GetRepository<EvaluationQuestion>()
                .SingleOrDefaultAsync(
                    predicate: q => q.Id == id,
                    include: q => q.Include(x => x.EvaluationOptions)
                );
            return question == null ? null : MapToResponse(question);
        }

        public async Task<EvaluationQuestionResponse> CreateQuestion(EvaluationQuestionRequest request)
        {
            var question = new EvaluationQuestion
            {
                Id = Guid.NewGuid(),
                QuestionText = request.QuestionText,
                QuestionTextLocalized = request.QuestionTextLocalized,
                QuestionType = request.QuestionType,
                Category = request.Category,
                OrderIndex = request.OrderIndex,
                IsActive = request.IsActive,
                EvaluationOptions = request.Options?.Select(o => new EvaluationOption
                {
                    Id = Guid.NewGuid(),
                    OptionText = o.OptionText
                }).ToList() ?? new List<EvaluationOption>()
            };
            await _unitOfWork.GetRepository<EvaluationQuestion>().InsertAsync(question);
            await _unitOfWork.CommitAsync();
            return MapToResponse(question);
        }

        public async Task<EvaluationQuestionResponse> UpdateQuestion(Guid id, EvaluationQuestionRequest request)
        {
            var repo = _unitOfWork.GetRepository<EvaluationQuestion>();
            var question = await repo.SingleOrDefaultAsync(
                predicate: q => q.Id == id,
                include: q => q.Include(x => x.EvaluationOptions)
            );
            if (question == null) return null;
            question.QuestionText = request.QuestionText;
            question.QuestionTextLocalized = request.QuestionTextLocalized;
            question.QuestionType = request.QuestionType;
            question.Category = request.Category;
            question.OrderIndex = request.OrderIndex;
            question.IsActive = request.IsActive;
            // Update options: remove old, add new
            question.EvaluationOptions.Clear();
            if (request.Options != null)
            {
                foreach (var o in request.Options)
                {
                    question.EvaluationOptions.Add(new EvaluationOption
                    {
                        Id = Guid.NewGuid(),
                        OptionText = o.OptionText
                    });
                }
            }
            repo.UpdateAsync(question);
            await _unitOfWork.CommitAsync();
            return MapToResponse(question);
        }

        public async Task<bool> DeleteQuestion(Guid id)
        {
            var repo = _unitOfWork.GetRepository<EvaluationQuestion>();
            var question = await repo.SingleOrDefaultAsync( predicate:q => q.Id == id);
            if (question == null) return false;
             repo.DeleteAsync(question);
            await _unitOfWork.CommitAsync();
            return true;
        }

        private static EvaluationQuestionResponse MapToResponse(EvaluationQuestion q)
        {
            return new EvaluationQuestionResponse
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                QuestionTextLocalized = q.QuestionTextLocalized,
                QuestionType = q.QuestionType,
                Category = q.Category,
                OrderIndex = q.OrderIndex,
                IsActive = q.IsActive,
                Options = q.EvaluationOptions?.Select(o => new EvaluationOptionResponse
                {
                    Id = o.Id,
                    OptionText = o.OptionText
                }).ToList() ?? new List<EvaluationOptionResponse>()
            };
        }
    }
} 