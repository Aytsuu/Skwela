using eSECAI.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSECAI.Domain.Entities;

/// <summary>
/// Posting Entity
/// Represents a classroom posting
/// </summary>
public class Posting
{
    public Guid post_id { get; set; }
    public string post_heading { get; set; } = default!;
    public string post_details { get; set; } = default!;
    public DateTime post_created_at { get; set; }
    public DateTime post_updated_at { get; set; }
    public Guid class_id { get; set; }

    [ForeignKey("class_id")]
    public Classroom? classroom { get; set; }

    /// <summary>
    /// Factory method for creating a new Posting with domain validation
    /// Validates both the classroom and user exist
    /// </summary>
    /// <param name=""></param>
    /// <returns>A new Posting instance</returns>
    /// <exception cref="DomainException">Thrown if </exception>
    public static Posting Build()
    {
        return new Posting
        {

        };
    }
}