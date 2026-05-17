namespace esecai.Domain.Entities;

public class UserReleaseRead
{
    public Guid user_release_read_id { get; set; } = Guid.NewGuid();
    public Guid user_id { get; set; }
    public Guid release_id { get; set; }
    public DateTime read_at { get; set; } = DateTime.UtcNow;

    public User? user { get; set; }
    public Release? release { get; set; }

    public static UserReleaseRead Build(Guid userId, Guid releaseId)
    {
        return new UserReleaseRead
        {
            user_id = userId,
            release_id = releaseId
        };
    }
}
