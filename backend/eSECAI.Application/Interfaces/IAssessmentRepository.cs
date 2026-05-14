using esecai.Domain.Entities;

public interface IAssessmentRepository
{
    Task<Assessment> CreateAssessmentAsync(Assessment assessment);
    Task<IEnumerable<Assessment>> GetAssessmentListAsync(Guid classId);
}