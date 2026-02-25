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
    /// <summary>
    /// Unique identifier for the user
    /// </summary>
    public Guid user_id { get; set; }
    
    /// <summary>
    /// Username for login (required for traditional auth)
    /// </summary>
    public string username { get; set; } = default!;
    
    /// <summary>
    /// Hashed password (only used for traditional signup/login)
    /// </summary>
    public string password { get; set; } = default!;
    
    /// <summary>
    /// Email address (required for Google OAuth)
    /// </summary>
    public string email { get; set; } = default!;
    
    /// <summary>
    /// Display name for the user interface
    /// </summary>
    public string display_name { get; set; } = default!;
    
    /// <summary>
    /// URL to the user's profile image
    /// Defaults to Cloudinary default profile image if not provided
    /// </summary>
    public string display_image { get; set; } = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png";
    
    /// <summary>
    /// User's role in the system (Teacher or Student)
    /// </summary>
    public UserRole role { get; set; }
    
    /// <summary>
    /// Timestamp when the user account was created
    /// </summary>
    public DateTime user_created_at { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// JWT refresh token for obtaining new access tokens
    /// </summary>
    public string? refreshToken { get; set; }
    
    /// <summary>
    /// Expiration time for the refresh token
    /// Tokens expire 7 days after generation
    /// </summary>
    public DateTime refreshTokenExpiryTime { get; set; }
    
    /// <summary>
    /// Navigation property: Classrooms created by this user (if teacher)
    /// </summary>
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
    public static User Build(string? email, string? username, string? password)
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
            display_name = username ?? email, // Use username if available, otherwise email
            display_image = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png",
            role = UserRole.student // Default role is student
        };
    }

}