using Skwela.Application.Interfaces;
using Skwela.Domain.Entities;


namespace Skwela.Application.UseCases.Classrooms;

public class CreateClassroomUseCase
{
    private readonly IClassroomRepository _repository;
    
    public CreateClassroomUseCase(IClassroomRepository repository)
    {
        _repository = repository;
    }

    public async Task<Classroom> ExecuteAsync(CreateClassroomDto dto)
    {
        var classroom = Classroom.Create( dto.userId, dto.name, dto.description);
        return await _repository.AddAsync(classroom);
    }
}