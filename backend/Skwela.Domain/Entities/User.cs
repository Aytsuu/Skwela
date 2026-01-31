using Skwela.Domain.Exceptions;
using Skwela.Domain.Enums;

namespace Skwela.Domain.Entities;

public class User
{   
    public Guid user_id { get; set; }
    public string username { get; set; } = default!;
    public string password { get; set; } = default!;
    public string email { get; set; } = default!;
    public string display_name { get; set; } = default!;
    public string display_image { get; set; } = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png";
    public UserRole role { get; set; }
    public DateTime user_created_at { get; set; } = DateTime.UtcNow;
    public string? refreshToken { get; set; }
    public DateTime refreshTokenExpiryTime { get; set; }
    public ICollection<Classroom> classrooms { get; set; } = new List<Classroom>();

    public static User Build(string? email, string? username, string? password)
    {
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
            display_name = username ?? email,
            display_image = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png",
            role = UserRole.student
        };
    }

}