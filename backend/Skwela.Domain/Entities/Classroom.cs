using Skwela.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skwela.Domain.Entities;

/// <summary>
/// Classroom Entity
/// Represents a classroom/course in the learning management system
/// Created and managed by teachers
/// </summary>
public class Classroom
{
    /// <summary>
    /// Unique identifier for the classroom
    /// </summary>
    public required Guid class_id { get; set; }
    
    /// <summary>
    /// Name of the classroom/course
    /// </summary>
    public string class_name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description of the classroom/course content
    /// </summary>
    public string class_description { get; set; } = string.Empty;
    
    /// <summary>
    /// Timestamp when the classroom was created
    /// </summary>
    public required DateTime class_created_at { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Foreign key: ID of the teacher who created this classroom
    /// </summary>
    public required Guid user_id { get; set; }

    /// <summary>
    /// Navigation property: The teacher who owns this classroom
    /// </summary>
    [ForeignKey("user_id")]
    public User? user { get; set; }

    /// <summary>
    /// Private constructor to enforce use of factory method
    /// </summary>
    private Classroom() { }

    /// <summary>
    /// Factory method for creating a new Classroom with domain validation
    /// Ensures classroom has a valid teacher (user)
    /// </summary>
    /// <param name=\"userId\">The ID of the teacher creating the classroom</param>
    /// <param name=\"name\">Name of the classroom</param>
    /// <param name=\"description\">Description of the classroom</param>
    /// <returns>A new Classroom instance with generated ID and timestamp</returns>
    /// <exception cref=\"DomainException\">Thrown if userId is empty (invalid teacher)</exception>
    public static Classroom Build(Guid userId, string? name, string? description)
    {
        // Business rule: A classroom must have a teacher
        if (userId == Guid.Empty)
            throw new DomainException(\"A classroom must have a teacher.\");

        // Create and return the classroom
        return new Classroom
        {
            class_id = Guid.NewGuid(),
            class_name = name ?? \"New Classroom\", // Default name if not provided
            class_description = description ?? string.Empty,
            class_created_at = DateTime.UtcNow,
            user_id = userId
        };
    }

}