using Skwela.Application.Interfaces;
using Skwela.Domain.Entities;

namespace Skwela.Application.UseCases.Enrollments;

/// <summary>
/// Use case for updating enrollment status
/// Handles toggling student enrollment status between active and inactive
/// </summary>
public class UpdateEnrollmentUseCase
{
    private readonly IEnrollmentRepository _repository;

    /// <summary>
    /// Initializes the UpdateEnrollmentUseCase with the enrollment repository
    /// </summary>
    /// <param name="repository">Repository for enrollment data operations</param>
    public UpdateEnrollmentUseCase(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Executes the enrollment status update
    /// Toggles the student's enrollment status between 'active' and 'inactive'
    /// Can be used to pause or resume a student's access to a classroom
    /// </summary>
    /// <param name="classId">The ID of the classroom</param>
    /// <param name="userId">The ID of the student</param>
    /// <returns>The updated Enrollment entity with new status</returns>
    /// <exception cref="KeyNotFoundException">Thrown if enrollment record is not found</exception>
    public async Task<Enrollment> ExecuteUpdateEnrollmentStatusAsync(Guid classId, Guid userId)
    {
        // Update enrollment status and toggle between active/inactive
        return await _repository.UpdateEnrollmentStatusAsync(classId, userId);
    }
}