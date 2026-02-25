using Microsoft.EntityFrameworkCore;
using Skwela.Infrastructure.Data;
using Skwela.Domain.Entities;
using Skwela.Application.Interfaces;

namespace Skwela.Infrastructure.Repositories;

/// <summary>
/// Enrollment Repository
/// Data access layer for student enrollment operations
/// Handles enrollment creation, retrieval, and status updates
/// </summary>
public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes the EnrollmentRepository with database context
    /// </summary>
    /// <param name="context">Entity Framework database context</param>
    public EnrollmentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new enrollment or reactivates an existing one
    /// If a student is already enrolled (even if inactive), the enrollment is reactivated
    /// Otherwise, a new enrollment record is created
    /// </summary>
    /// <param name="enrollment">Enrollment entity to be created or reactivated</param>
    /// <returns>The enrollment record (newly created or reactivated)</returns>
    public async Task<Enrollment> AddEnrollmentAsync(Enrollment enrollment)
    {
        // Check if student is already enrolled in this classroom
        var alreadyEnrolled =  await _context.Enrollments
            .AnyAsync(e => e.class_id == enrollment.class_id && e.user_id == enrollment.user_id);

        // If already enrolled, reactivate the enrollment
        if (alreadyEnrolled == true)
        {
            return await UpdateEnrollmentStatusAsync(enrollment.class_id, enrollment.user_id);
        } 
        // Otherwise create new enrollment
        else
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }
        
        return enrollment;
    }

    /// <summary>
    /// Retrieves all active enrollments for a specific student
    /// Returns detailed information about each classroom the student is enrolled in
    /// </summary>
    /// <param name="userId">The ID of the student</param>
    /// <returns>Collection of active Enrollment entities with classroom information</returns>
    public async Task<IEnumerable<Enrollment>> GetStudentEnrollmentAsync(Guid userId)
    {
        // Query active enrollments for the student with classroom details
        return await _context.Enrollments
            .Where(e => e.user_id == userId && e.enrolled_status == "active")
            .Include(e => e.classroom)
            .ToListAsync();
    }

    /// <summary>
    /// Updates the enrollment status for a student in a classroom
    /// Toggles between 'active' and 'inactive' states
    /// </summary>
    /// <param name="classId">The ID of the classroom</param>
    /// <param name="userId">The ID of the student</param>
    /// <returns>The updated Enrollment entity with new status</returns>
    /// <exception cref="KeyNotFoundException">Thrown if enrollment record is not found</exception>
    public async Task<Enrollment> UpdateEnrollmentStatusAsync(Guid classId, Guid userId)
    {
        // Find the enrollment record
        var enrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.class_id == classId && e.user_id == userId);

        if (enrollment == null)
        {
            throw new KeyNotFoundException("Student is not enrolled in this class.");
        }

        // Toggle enrollment status between active and inactive
        enrollment.enrolled_status = enrollment.enrolled_status == "active" ? "inactive" : "active";
        await _context.SaveChangesAsync();

        return enrollment;
    }
}