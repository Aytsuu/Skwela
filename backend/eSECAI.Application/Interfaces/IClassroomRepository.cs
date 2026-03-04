using eSECAI.Domain.Entities;

namespace eSECAI.Application.Interfaces;

public interface IClassroomRepository
{
    Task<Classroom> AddAsync(Classroom classroom);
    Task<IEnumerable<Classroom>> GetClassroomsByUserIdAsync(Guid userId);
    Task<Classroom> GetClassroomDataAsync(Guid classId, Guid userId);
    Task RemoveClassroomAsync(Guid classId);
}
