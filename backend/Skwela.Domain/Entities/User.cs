using Skwela.Domain.Exceptions;
using Skwela.Domain.Enums;

namespace Skwela.Domain.Entities;

/// <summary>
/// User Entity
/// Represents a user in the learning management system
/// Can be either a teacher or a student
/// </summary>
public class User
{   
    public Guid user_id { get; set; }
    public string username { get; set; } = default!;
    public string password { get; set; } = default!;
    public string email { get; set; } = default!;
    public string display_name { get; set; } = default!;
    public string display_image { get; set; } = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png";
    public UserRole role { get; set; }
    public bool is_email_verified { get; set; } = false;
    public DateTime user_created_at { get; set; } = DateTime.UtcNow;
    public string? refreshToken { get; set; }
    public DateTime refreshTokenExpiryTime { get; set; }
    public ICollection<Classroom> classrooms { get; set; } = new List<Classroom>();

    /// <summary>
    /// Factory method for creating a new User with domain validation
    /// Guards against invalid data during object creation
    /// </summary>
    /// <param name="email">Email address (can be null if username is provided)</param>
    /// <param name="username">Username (can be null if email is provided)</param>
    /// <param name="password">Hashed password</param>
    /// <returns>A new User instance with default values</returns>
    /// <exception cref="DomainException">Thrown if neither email nor username is provided</exception>
    public static User Build(string name, string? email, string? username, string? password, bool? is_email_verified)
    {
        // Business rule: User must have either email or username
        if (string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException("Either email or username must be filled.");
        }
        
        return new User
        {
            user_id = Guid.NewGuid(),
            username = username ?? "",
            email = email ?? "", 
            password = password ?? "",
            display_name = name,
            display_image = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png",
            role = UserRole.student, // Default role is student
            is_email_verified = is_email_verified ?? false
        };
    }

}