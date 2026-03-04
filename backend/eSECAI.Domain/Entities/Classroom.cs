using eSECAI.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSECAI.Domain.Entities;

/// <summary>
/// Classroom Entity
/// Represents a classroom/course in the learning management system
/// Created and managed by teachers
/// </summary>
public class Classroom
{
    public Guid class_id { get; set; }
    public string class_name { get; set; } = default!;
    public string class_description { get; set; } = default!;
    public string class_image { get; set; } = default!;
    public bool class_is_archived { get; set; } = false;
    public DateTime class_created_at { get; set; }
    public DateTime class_updated_at { get; set; }
    public Guid user_id { get; set; }

    [ForeignKey("user_id")]
    public User? user { get; set; }

    /// <summary>
    /// Factory method for creating a new Classroom with domain validation
    /// Ensures classroom has a valid teacher (user)
    /// </summary>
    /// <param name=\"userId\">The ID of the teacher creating the classroom</param>
    /// <param name=\"name\">Name of the classroom</param>
    /// <param name=\"description\">Description of the classroom</param>
    /// <returns>A new Classroom instance with generated ID and timestamp</returns>
    /// <exception cref=\"DomainException\">Thrown if userId is empty (invalid teacher)</exception>
    public static Classroom Build(
        Guid userId, 
        string? name, 
        string? description, 
        string? image
    )
    {
        // Business rule: A classroom must have a teacher
        if (userId == Guid.Empty)
            throw new DomainException("A classroom must have creator.");

        // Create and return the classroom 
        return new Classroom
        {
            class_id = Guid.NewGuid(),
            class_name = name ?? "New Classroom", // Default name if not provided
            class_description = description ?? string.Empty,
            class_image = image ?? string.Empty,
            user_id = userId
        };
    }

}