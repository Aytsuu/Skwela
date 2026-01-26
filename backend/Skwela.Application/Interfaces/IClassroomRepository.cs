
using Skwela.Domain.Entities;
using Skwela.Domain.Enums;

namespace Skwela.Application.Interfaces;

public interface IClassroomRepository
{
    Task<Classroom> AddAsync(Classroom classroom);
    Task<IEnumerable<Classroom>> GetClassroomsByUserIdAsync(Guid userId);
    Task<Classroom> GetClassroomDataAsync(Guid classId, Guid userId, UserRole role);
    Task RemoveClassroomAsync(Guid classId);
}
