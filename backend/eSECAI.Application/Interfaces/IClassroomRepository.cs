using esecai.Domain.Entities;

namespace esecai.Application.Interfaces;

public interface IClassroomRepository
{
    Task<Classroom> AddAsync(Classroom classroom);
    Task<IEnumerable<Classroom>> GetClassroomsByCreatorAsync(Guid userId);
    Task<Classroom> GetClassroomDataAsync(Guid classId);
    Task<Student> AddStudentAsync(Student student);
    Task<IReadOnlyList<Student>> AddStudentsAsync(IEnumerable<Student> students);
    Task<Student> GetStudentAsync(Guid classId, Guid studentId);
    Task<bool> StudentExistsAsync(Guid classId, string firstName, string middleName, string lastName, Guid? excludeStudentId = null);
    Task UpdateClassroomAsync();
    Task DeleteStudentAsync(Student student);
    Task DeleteClassroomAsync(Classroom classroom);
}
