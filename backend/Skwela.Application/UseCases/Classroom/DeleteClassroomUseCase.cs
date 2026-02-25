using Skwela.Application.Interfaces;
using Skwela.Domain.Entities;

namespace Skwela.Application.UseCases.Classrooms;

/// <summary>
/// Use case for deleting classrooms
/// Ensures that classrooms with active students cannot be deleted
/// </summary>
public class DeleteClassroomUseCase
{
    /// <summary>
    /// Repository for classroom data operations
    /// </summary>
    private readonly IClassroomRepository _repository;

    /// <summary>
    /// Initializes the DeleteClassroomUseCase with the classroom repository
    /// </summary>
    /// <param name="repository">Repository for classroom data operations</param>
    public DeleteClassroomUseCase(IClassroomRepository repository) 
    {
        _repository = repository;
    }

    /// <summary>
    /// Executes the classroom deletion process
    /// A classroom can only be deleted if it has no active enrolled students
    /// </summary>
    /// <param name="classId">The ID of the classroom to delete</param>
    /// <returns>Async task that completes when deletion is successful</returns>
    /// <exception cref="InvalidOperationException">Thrown if classroom has active students enrolled</exception>
    public async Task ExecuteRemoveClassroomAsync(Guid classId)
    {
        // Delete the classroom - repository will check for active enrollments
        await _repository.RemoveClassroomAsync(classId);
    }
}