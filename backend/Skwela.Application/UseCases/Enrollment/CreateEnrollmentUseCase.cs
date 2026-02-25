using Skwela.Application.Interfaces;
using Skwela.Domain.Entities;

namespace Skwela.Application.UseCases.Enrollments;

/// <summary>
/// Use case for creating student enrollments in classrooms
/// Handles validation and persistence of enrollment records
/// </summary>
public class CreateEnrollmentUseCase
{
    private readonly IEnrollmentRepository _repository;

    /// <summary>
    /// Initializes the CreateEnrollmentUseCase with the enrollment repository
    /// </summary>
    /// <param name="repository">Repository for enrollment data operations</param>
    public CreateEnrollmentUseCase(IEnrollmentRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Executes the enrollment creation process
    /// Validates enrollment data through domain entity and persists to database
    /// If student is already enrolled but inactive, reactivates the enrollment
    /// </summary>
    /// <param name="dto">CreateEnrollmentDto containing classId and userId</param>
    /// <returns>The Enrollment entity (newly created or reactivated)</returns>
    /// <exception cref="DomainException">Thrown if enrollment data fails domain validation</exception>
    public Task<Enrollment> ExecuteAsync(CreateEnrollmentDto dto)
    {
        // Create enrollment with domain validation
        var enrollment = Enrollment.Build(dto.classId, dto.userId);
        
        // Persist to database - repository handles duplicate enrollment logic
        return _repository.AddEnrollmentAsync(enrollment);
    }
}