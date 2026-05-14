using Microsoft.EntityFrameworkCore;
using esecai.Application.Interfaces;
using esecai.Infrastructure.Data;
using esecai.Domain.Entities;

namespace esecai.Infrastructure.Repositories;

public class AssessmentRepository : IAssessmentRepository
{   
    private readonly AppDbContext _context;

    public AssessmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Assessment> CreateAssessmentAsync(Assessment assessment)
    {
        _context.Assessments.Add(assessment);
        await _context.SaveChangesAsync();

        return assessment;
    }

    public async Task<IEnumerable<Assessment>> GetAssessmentListAsync(Guid classId)
    {
        return await _context.Assessments
            .Include(e => e.classroom)
            .Where(e => e.class_id == classId)
            .ToListAsync();
    }
}