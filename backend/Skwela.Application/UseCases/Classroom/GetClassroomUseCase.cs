using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;
using Skwela.Domain.Enums;

namespace Skwela.Application.UseCases.Classrooms;

/// <summary>
/// Use case for retrieving classroom information
/// Handles both listing classrooms and fetching detailed information with authorization checks
/// </summary>
public class GetClassroomUseCase
{
    private readonly IClassroomRepository _repository;

    /// <summary>
    /// Initializes the GetClassroomUseCase with the classroom repository
    /// </summary>
    /// <param name="repository">Repository for classroom data operations</param>
    public GetClassroomUseCase(IClassroomRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Retrieves all classrooms created by a specific user (teacher)
    /// </summary>
    /// <param name="userId">The ID of the user whose classrooms to retrieve</param>
    /// <returns>Collection of Classroom entities created by the user</returns>
    public async Task<IEnumerable<Classroom>> ExecuteGetByUserIdAsync(Guid userId)
    {
        return await _repository.GetClassroomsByUserIdAsync(userId);
    }

    /// <summary>
    /// Retrieves detailed information about a specific classroom
    /// Performs authorization checks based on user role:
    /// - Teachers can only access their own classrooms
    /// - Students can only access classrooms they are enrolled in
    /// </summary>
    /// <param name="classId">The ID of the classroom to retrieve</param>
    /// <param name="userId">The ID of the user requesting access</param>
    /// <param name="role">The role of the user (teacher or student)</param>
    /// <returns>ClassroomResponse with classroom details and teacher information</returns>
    /// <exception cref="KeyNotFoundException">Thrown if classroom does not exist</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if user is not authorized to access the classroom</exception>
    public async Task<ClassroomResponse> ExecuteGetClassroomDataAsync(Guid classId, Guid userId, UserRole role)
    {
        // Retrieve classroom with authorization checks
        var classData = await _repository.GetClassroomDataAsync(classId, userId, role);

        if (classData == null)
        {
            throw new KeyNotFoundException("Classroom not found");
        }

        // Build and return response DTO
        return new ClassroomResponse(
            classData.class_id,
            classData.class_name,
            classData.class_description,
            classData.class_created_at,
            classData.user?.display_name ?? "Unknown Teacher",
            classData.user?.display_image ?? string.Empty
        );
    }
}