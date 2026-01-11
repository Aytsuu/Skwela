using Skwela.Domain.Exceptions;
using Skwela.Domain.Enums;

namespace Skwela.Domain.Entities;

public class User
{   
    public Guid user_id { get; set; }
    public required string username { get; set; }
    public string password { get; set; } = default!;
    public string display_name { get; set; } = default!;
    public string display_image { get; set; } = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png";
    public UserRole role { get; set; }
    public DateTime user_created_at { get; set; } = DateTime.UtcNow;
    public string? refreshToken { get; set; }
    public DateTime refreshTokenExpiryTime { get; set; }
    public ICollection<Classroom> classrooms { get; set; } = new List<Classroom>();

    public static User Create(string username, string password)
    {
        if(string.IsNullOrWhiteSpace(username))
        {
            throw new DomainException("Username cannot be empty.");
        }

        if(string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("Password cannot be empty.");
        }

        return new User
        {
            user_id = Guid.NewGuid(),
            username = username,
            password = password,
            display_name = username,
            display_image = "https://res.cloudinary.com/dzcmadjl1/image/upload/v1694868283/default_profile_image_oqxv6r.png",
            role = UserRole.Student
        };
    }

}