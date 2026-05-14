using esecai.Application.Interfaces;
using esecai.Application.DTOs;

namespace esecai.Application.UseCases.Assessments;

public class GetAssessmentUseCase
{
    private readonly IAssessmentRepository _repository;

    public GetAssessmentUseCase(IAssessmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AssessmentData>> ExecuteGetAssessmentListAsync(Guid classId)
    {
        var assessments = await _repository.GetAssessmentListAsync(classId);

        return assessments
            .Select(e => new AssessmentData(
                e.ass_id,
                e.ass_title,
                e.ass_type,
                e.ass_total_points,
                e.ass_status,
                e.ass_created_at,
                e.ass_updated_at
            ));
    }
}
