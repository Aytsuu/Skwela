using Microsoft.Extensions.Logging;
using Skwela.Application.Interfaces;
using Skwela.Infrastructure.Data;
using Skwela.Domain.Entities;
using Skwela.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace Skwela.Infrastructure.Repositories;

public class ClassroomRepository : IClassroomRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ClassroomRepository> _logger;

    public ClassroomRepository(AppDbContext context, ILogger<ClassroomRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Classroom> AddAsync(Classroom classroom)
    {

        _context.Classrooms.Add(classroom);
        await _context.SaveChangesAsync();

        return classroom;
    }

    public async Task<IEnumerable<Classroom>> GetClassroomsByUserIdAsync(Guid userId)
    {
        return await _context.Classrooms
            .Where(c => c.user_id == userId)
            .ToListAsync();
    }

    public async Task<Classroom> GetClassroomDataAsync(Guid classId, Guid userId, UserRole role)
    {
        var classroom = await _context.Classrooms
            .Include(c => c.user)
            .FirstOrDefaultAsync(c => c.class_id == classId);

        if (classroom == null)
        {
            throw new KeyNotFoundException("Classroom not found.");
        }

        if (role == UserRole.teacher && classroom.user_id != userId) // Applies to teachers
        {
            throw new UnauthorizedAccessException("You do not have access to this classroom.");
        }
        else if(role == UserRole.student) // Applies to students
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

    public async Task RemoveClassroomAsync(Guid classId)
    {
        var hasStudent = await _context.Enrollments
            .AnyAsync(e => e.class_id == classId && e.enrolled_status == "active");

        if (hasStudent)
        {
            throw new InvalidOperationException("Cannot remove a class.");
        }

        await _context.Classrooms
            .Where(e => e.class_id == classId)
            .ExecuteDeleteAsync();
    }

}