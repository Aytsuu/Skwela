using Skwela.Application.Interfaces;

namespace Skwela.Application.UseCases.Enrollments;

/// <summary>
/// Use case for retrieving student enrollment information
/// Handles fetching all active enrollments for a specific student
/// </summary>
public class GetEnrollmentUseCase
{
    /// <summary>
    /// Repository for enrollment data operations
    /// </summary>
    private IEnrollmentRepository _repository;

    /// <summary>
    /// Initializes the GetEnrollmentUseCase with the enrollment repository
    /// </summary>
    /// <param name="repository">Repository for enrollment data operations</param>
    public GetEnrollmentUseCase(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves all active enrollments for a specific student
    /// Returns detailed information about each classroom the student is enrolled in
    /// </summary>
    /// <param name="userId">The ID of the student</param>
    /// <returns>Collection of EnrollmentDto objects with classroom details</returns>
    public async Task<IEnumerable<EnrollmentDto>> ExecuteGetStudentEnrollmentAsync(Guid userId)
    {
        // Retrieve student's active enrollments from database
        var enrollments = await _repository.GetStudentEnrollmentAsync(userId);

        // Map enrollment entities to DTOs with classroom information
        return enrollments.Select(e => new EnrollmentDto(
            e.class_id,
            e.classroom?.class_name ?? "Unknown Classroom",
            e.classroom?.class_description ?? string.Empty,
            e.enrolled_at
        ));
    }
}