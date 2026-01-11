using Microsoft.Extensions.Logging;
using Skwela.Application.Interfaces;
using Skwela.Infrastructure.Data;
using Skwela.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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

}