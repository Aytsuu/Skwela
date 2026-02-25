using Skwela.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skwela.Domain.Entities;

/// <summary>
/// Enrollment Entity
/// Represents a student's enrollment in a classroom
/// Maintains the relationship between students and classrooms
/// </summary>
public class Enrollment
{
    /// <summary>
    /// Foreign key: ID of the classroom the student is enrolled in
    /// </summary>
    public Guid class_id { get; set; }
    
    /// <summary>
    /// Foreign key: ID of the student enrolled
    /// </summary>
    public Guid user_id { get; set; }
    
    /// <summary>
    /// Timestamp when the student was enrolled
    /// </summary>
    public DateTime enrolled_at { get; set; }
    
    /// <summary>
    /// Status of the enrollment: "active" or "inactive"
    /// Allows students to pause/resume access without fully unenrolling
    /// </summary>
    public string enrolled_status { get; set; } = "active";

    /// <summary>
    /// Navigation property: The classroom the student is enrolled in
    /// </summary>
    [ForeignKey("class_id")]
    public Classroom? classroom { get; set; }
    
    /// <summary>
    /// Navigation property: The student who is enrolled
    /// </summary>
    [ForeignKey("user_id")]
    public User? user { get; set; }

    /// <summary>
    /// Factory method for creating a new Enrollment with domain validation
    /// Validates both the classroom and user exist
    /// </summary>
    /// <param name="classId">The ID of the classroom</param>
    /// <param name="userId">The ID of the student</param>
    /// <returns>A new Enrollment instance with generated timestamp</returns>
    /// <exception cref="DomainException">Thrown if classId or userId is empty (invalid)</exception>
    public static Enrollment Build(Guid classId, Guid userId)
    {
        // Business rule: Enrollment must have a valid classroom
        if (classId == Guid.Empty)
            throw new DomainException("Enrollment must have a valid class ID.");
        
        // Business rule: Enrollment must have a valid student
        if (userId == Guid.Empty)
            throw new DomainException("Enrollment must have a valid user ID.");

        // Create and return the enrollment
        return new Enrollment
        {
            class_id = classId,
            user_id = userId,
            enrolled_at = DateTime.UtcNow,
            enrolled_status = "active" // New enrollments are active by default
        };
    }
}