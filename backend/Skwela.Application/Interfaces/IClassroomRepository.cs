
using Skwela.Domain.Entities;

namespace Skwela.Application.Interfaces;

public interface IClassroomRepository
{
    Task<Classroom> AddAsync(Classroom classroom);
    Task<IEnumerable<Classroom>> GetClassroomsByUserIdAsync(Guid userId);
}
