using eSECAI.Domain.Exceptions;
using System.ComponentModel.DataAnnotations.Schema;

namespace eSECAI.Domain.Entities;

/// <summary>
/// Submission Entity
/// Represents a classroom Submission
/// Maintains the relationship between user and posting
/// </summary>
public class Submission
{
    public Guid sub_id { get; set; }
    public float sub_score { get; set; } = 0;
    public string sub_remark { get; set; } = default!;
    public DateTime sub_created_at { get; set; }
    public Guid user_id { get; set; }
    public Guid post_id { get; set; }

    [ForeignKey("user_id")]
    public User? user { get; set; }

    [ForeignKey("post_id")]
    public Posting? posting { get; set; }

    /// <summary>
    /// Factory method for creating a new Submission with domain validation
    /// Validates both the classroom and user exist
    /// </summary>
    /// <param name=""></param>
    /// <returns>A new Submission instance</returns>
    /// <exception cref="DomainException">Thrown if </exception>
    public static Submission Build()
    {
        return new Submission
        {

        };
    }
}