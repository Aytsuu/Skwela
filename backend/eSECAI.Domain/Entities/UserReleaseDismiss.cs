namespace esecai.Domain.Entities;

public class UserReleaseDismiss
{
    public Guid user_release_dismiss_id { get; set; } = Guid.NewGuid();
    public Guid user_id { get; set; }
    public Guid release_id { get; set; }
    public DateTime dismissed_at { get; set; } = DateTime.UtcNow;

    public User? user { get; set; }
    public Release? release { get; set; }

    public static UserReleaseDismiss Build(Guid userId, Guid releaseId)
    {
        return new UserReleaseDismiss
        {
            user_id = userId,
            release_id = releaseId
        };
    }
}
