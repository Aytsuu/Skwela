using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;

namespace Skwela.Application.UseCases.Classrooms;

public class GetClassroomUseCase
{
    private readonly IClassroomRepository _repository;

    public GetClassroomUseCase(IClassroomRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Classroom>> ExecuteGetByUserIdAsync(Guid userId)
    {
        return await _repository.GetClassroomsByUserIdAsync(userId);
    }
}