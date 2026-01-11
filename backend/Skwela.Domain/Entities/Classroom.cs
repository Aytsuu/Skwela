using Skwela.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skwela.Domain.Entities;

public class Classroom
{
    public Guid class_id { get; set; }
    public string class_name { get; set; } = string.Empty;
    public string class_description { get; set; } = string.Empty;
    public DateTime class_created_at { get; set; } = DateTime.UtcNow;
    // Foreign Key
    public Guid user_id { get; set; }

    [ForeignKey("user_id")]
    public User? user { get; set; }

    private Classroom() { }

    public static Classroom Create(Guid userId, string? name, string? description)
    {
        // Validation (Guard Clauses
        if (userId == Guid.Empty)
            throw new DomainException("A classroom must have a teacher.");

        // Creation
        return new Classroom
        {
            class_id = Guid.NewGuid(),
            class_name = name ?? "New Classroom",
            class_description = description ?? string.Empty,
            class_created_at = DateTime.UtcNow,
            user_id = userId
        };
    }

}