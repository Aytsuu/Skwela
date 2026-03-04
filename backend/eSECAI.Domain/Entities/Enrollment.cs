using eSECAI.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSECAI.Domain.Entities;

/// <summary>
/// Enrollment Entity
/// Represents a student's enrollment in a classroom
/// Maintains the relationship between students and classrooms
/// </summary>
public class Enrollment
{
    public Guid enroll_id { get; set; }
    public bool enroll_is_approved { get; set; } = false;
    public DateTime enroll_created_at { get; set; }
    public Guid class_id { get; set; }
    public Guid user_id { get; set; }
    
    [ForeignKey("class_id")]
    public Classroom? classroom { get; set; }
    
    [ForeignKey("user_id")]
    public User? user { get; set; }

    /// <summary>
    /// Factory method for creating a new Enrollment with domain validation
    /// Validates both the classroom and user exist
    /// </summary>
    /// <param name="classId">The ID of the classroom</param>
    /// <param name="userId">The ID of the student</param>
    /// <returns>A new Enrollment instance</returns>
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
            enroll_id = Guid.NewGuid(),
            class_id = classId,
            user_id = userId
        };
    }
}