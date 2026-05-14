using esecai.Domain.Entities;

namespace esecai.Application.Interfaces;

public interface IQuestionRepository
{
	Task<Question> CreateQuestionAsync(Question question);
	Task<IEnumerable<Question>> CreateQuestionsBulkAsync(IEnumerable<Question> questions);
	Task<IEnumerable<Question>> GetAssessmentQuestionsAsync(Guid assId);
}