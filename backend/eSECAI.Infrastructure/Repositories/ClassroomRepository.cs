using Microsoft.Extensions.Logging;
using esecai.Application.Interfaces;
using esecai.Infrastructure.Data;
using esecai.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace esecai.Infrastructure.Repositories;

/// <summary>
/// Classroom Repository
/// Data access layer for classroom operations
/// Handles CRUD operations and authorization checks for classroom access
/// </summary>
public class ClassroomRepository : IClassroomRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClassroomRepository> _logger;

    /// <summary>
    /// Initializes the ClassroomRepository with database context and logger
    /// </summary>
    /// <param name="context">Entity Framework database context</param>
    /// <param name="logger">Logger for debugging and error tracking</param>
    public ClassroomRepository(AppDbContext context, ILogger<ClassroomRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates and persists a new classroom to the database
    /// </summary>
    /// <param name="classroom">Classroom entity to be created</param>
    /// <returns>The created classroom entity with database-generated ID</returns>
    public async Task<Classroom> AddAsync(Classroom classroom)
    {
        // Add classroom to context and persist to database
        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        return classroom;
    }

    /// <summary>
    /// Retrieves all classrooms created by a specific user (teacher)
    /// </summary>
    /// <param name="userId">The ID of the teacher who created the classrooms</param>
    /// <returns>Collection of Classroom entities created by the user</returns>
    public async Task<IEnumerable<Classroom>> GetClassroomsByCreatorAsync(Guid userId)
    {
        // Query classrooms by creator user_id
        return await _context.Classrooms
            .Include(c => c.user)
            .Include(c => c.students)
            .Where(c => c.user_id == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves detailed classroom information with authorization checks
    /// </summary>
    /// <param name="classId">The ID of the classroom to retrieve</param>
    /// <returns>The Classroom entity with included teacher information</returns>
    /// <exception cref="KeyNotFoundException">Thrown if classroom does not exist</exception>
    public async Task<Classroom> GetClassroomDataAsync(Guid classId)
    {
        // Load classroom with creator information
        var classroom = await _context.Classrooms
            .Include(c => c.user)
            .Include(c => c.students)
            .FirstOrDefaultAsync(c => c.class_id == classId);

        if (classroom == null)
        {
            throw new KeyNotFoundException("Classroom not found.");
        }
        
        return classroom;
    }

    public async Task<Student> AddStudentAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return student;
    }

    public async Task<IReadOnlyList<Student>> AddStudentsAsync(IEnumerable<Student> students)
    {
        var studentList = students.ToList();
        _context.Students.AddRange(studentList);
        await _context.SaveChangesAsync();

        return studentList;
    }

    public async Task<Student> GetStudentAsync(Guid classId, Guid studentId)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.class_id == classId && s.student_id == studentId);

        if (student == null)
        {
            throw new KeyNotFoundException("Student not found.");
        }

        return student;
    }

    public async Task<bool> StudentExistsAsync(
        Guid classId,
        string firstName,
        string middleName,
        string lastName,
        Guid? excludeStudentId = null)
    {
        return await _context.Students.AnyAsync(student =>
            student.class_id == classId &&
            student.student_fname == firstName &&
            student.student_mname == middleName &&
            student.student_lname == lastName &&
            (!excludeStudentId.HasValue || student.student_id != excludeStudentId.Value));
    }

    /// <summary>
    /// Update classroom data
    /// </summary>
    public async Task UpdateClassroomAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task DeleteStudentAsync(Student student)
    {
        _context.Students.Remove(student);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes a classroom from the system
    /// </summary>
    /// <param name="classroom">The classroom entity to delete</param>
    public async Task DeleteClassroomAsync(Classroom classroom)
    {
        // Delete classroom from database
        _context.Classrooms.Remove(classroom);
        await _context.SaveChangesAsync();
    }
}
