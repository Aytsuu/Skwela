using esecai.Application.Interfaces;
using esecai.Application.DTOs;

namespace esecai.Application.UseCases.Questions;

public class GetQuestionUseCase
{
    private readonly IQuestionRepository _repository;

    public GetQuestionUseCase(IQuestionRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<QuestionData>> ExecuteGetAssessmentQuestionsAsync(Guid assId)
    {
        var questions = await _repository.GetAssessmentQuestionsAsync(assId);

        return questions
            .Select(e => new QuestionData(
                e.quest_id,
                e.quest_num,
                e.quest_type,
                e.quest_text,
                e.quest_correct_answer,
                e.quest_rubric,
                e.quest_max_points,
                e.quest_ai_confidence
            ));
    }
}