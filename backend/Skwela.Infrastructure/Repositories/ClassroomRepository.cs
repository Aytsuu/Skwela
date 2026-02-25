using Microsoft.Extensions.Logging;
using Skwela.Application.Interfaces;
using Skwela.Infrastructure.Data;
using Skwela.Domain.Entities;
using Skwela.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace Skwela.Infrastructure.Repositories;

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
    public async Task<IEnumerable<Classroom>> GetClassroomsByUserIdAsync(Guid userId)
    {
        // Query classrooms where teacher is the owner
        return await _context.Classrooms
            .Where(c => c.user_id == userId)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves detailed classroom information with authorization checks
    /// Different access rules apply based on user role:
    /// - Teachers can only access their own classrooms
    /// - Students can only access classrooms they are enrolled in
    /// </summary>
    /// <param name="classId">The ID of the classroom to retrieve</param>
    /// <param name="userId">The ID of the user requesting access</param>
    /// <param name="role">The role of the user (teacher or student)</param>
    /// <returns>The Classroom entity with included teacher information</returns>
    /// <exception cref="KeyNotFoundException">Thrown if classroom does not exist</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown if user is not authorized to access the classroom</exception>
    public async Task<Classroom> GetClassroomDataAsync(Guid classId, Guid userId, UserRole role)
    {
        // Load classroom with teacher information
        var classroom = await _context.Classrooms
            .Include(c => c.user)
            .FirstOrDefaultAsync(c => c.class_id == classId);

        if (classroom == null)
        {
            throw new KeyNotFoundException("Classroom not found.");
        }

        // Authorization check for teachers - can only access their own classrooms
        if (role == UserRole.teacher && classroom.user_id != userId)
        {
            throw new UnauthorizedAccessException("You do not have access to this classroom.");
        }
        // Authorization check for students - must be enrolled in the classroom
        else if(role == UserRole.student)
        {
            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.class_id == classId && e.user_id == userId);

            if (!isEnrolled)
            {
                throw new UnauthorizedAccessException("You are not enrolled in this classroom.");
            }
        }

        return classroom;
    }

    /// <summary>
    /// Deletes a classroom from the system
    /// A classroom can only be deleted if it has no active enrolled students
    /// </summary>
    /// <param name="classId">The ID of the classroom to delete</param>
    /// <exception cref="InvalidOperationException">Thrown if classroom has active student enrollments</exception>
    public async Task RemoveClassroomAsync(Guid classId)
    {
        // Check if classroom has any active enrollments
        var hasStudent = await _context.Enrollments
            .AnyAsync(e => e.class_id == classId && e.enrolled_status == "active");

        if (hasStudent)
        {
            throw new InvalidOperationException("Cannot remove a class.");
        }

        // Delete classroom from database
        await _context.Classrooms
            .Where(e => e.class_id == classId)
            .ExecuteDeleteAsync();
    }
}