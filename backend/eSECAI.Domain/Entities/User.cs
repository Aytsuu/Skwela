using eSECAI.Domain.Exceptions;

namespace eSECAI.Domain.Entities;

/// <summary>
/// User Entity
/// Represents a user in the learning management system
/// Can be either a teacher or a student
/// </summary>
public class User
{   
    public Guid user_id { get; set; }
    public string email { get; set; } = default!;
    public string password { get; set; } = default!;
    public string display_name { get; set; } = default!;
    public string display_image { get; set; } = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png";
    public bool is_admin { get; set; } = false;
    public bool is_email_verified { get; set; } = false;
    public DateTime user_created_at { get; set; }
    public DateTime user_updated_at { get; set; } 
    public string? refreshToken { get; set; }
    public DateTime refreshTokenExpiryTime { get; set; }
    public ICollection<Classroom> classrooms { get; set; } = new List<Classroom>();

    /// <summary>
    /// Factory method for creating a new User with domain validation
    /// Guards against invalid data during object creation
    /// </summary>
    /// <param name="name">User display name</param>
    /// <param name="email">Email address (can be null if username is provided)</param>
    /// <param name="password">Hashed password</param>
    /// <param name="is_email_verified">Email verified status</param>
    /// <returns>A new User instance with default values</returns>
    /// <exception cref="DomainException">Thrown if neither email nor username is provided</exception>
    public static User Build(string name, string? email, string? password, bool? is_email_verified)
    {
        // Business rule: User must have either email or username
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email must be filled.");
        }
        
        return new User
        {
            user_id = Guid.NewGuid(),
            email = email ?? "", 
            password = password ?? "",
            display_name = name,
            display_image = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png",
            is_email_verified = is_email_verified ?? false
        };
    }

}