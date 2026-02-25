using Skwela.Application.Interfaces;
using Skwela.Domain.Entities;

namespace Skwela.Application.UseCases.Classrooms;

/// <summary>
/// Use case for creating new classrooms
/// Handles validation and persistence of classroom data
/// </summary>
public class CreateClassroomUseCase
{
    private readonly IClassroomRepository _repository;
    
    /// <summary>
    /// Initializes the CreateClassroomUseCase with the classroom repository
    /// </summary>
    /// <param name="repository">Repository for classroom data operations</param>
    public CreateClassroomUseCase(IClassroomRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Executes the classroom creation process
    /// Validates classroom data through domain entity and persists to database
    /// </summary>
    /// <param name="dto">CreateClassroomDto containing userId, classroom name, and description</param>
    /// <returns>The newly created Classroom entity with generated ID and timestamp</returns>
    /// <exception cref="DomainException">Thrown if classroom data fails domain validation</exception>
    public async Task<Classroom> ExecuteAsync(CreateClassroomDto dto)
    {
        // Create classroom with domain validation
        var classroom = Classroom.Build(dto.userId, dto.name, dto.description);
        
        // Persist to database
        return await _repository.AddAsync(classroom);
    }
}