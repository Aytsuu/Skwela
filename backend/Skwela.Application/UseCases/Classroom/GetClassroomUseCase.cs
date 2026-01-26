using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;
using Skwela.Domain.Enums;

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

    public async Task<ClassroomResponse> ExecuteGetClassroomDataAsync(Guid classId, Guid userId, UserRole role)
    {
        var classData = await _repository.GetClassroomDataAsync(classId, userId, role);

        if (classData == null)
        {
            throw new KeyNotFoundException("Classroom not found");
        }

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