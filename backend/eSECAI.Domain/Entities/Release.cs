namespace esecai.Domain.Entities;

public class Release
{
    public Guid release_id { get; set; } = Guid.NewGuid();
    public string? release_version { get; set; }
    public string release_title { get; set; } = string.Empty;
    public string release_summary { get; set; } = string.Empty;
    public string release_body { get; set; } = string.Empty;
    public string release_status { get; set; } = "draft";
    public DateTime? release_published_at { get; set; }
    public DateTime release_created_at { get; set; } = DateTime.UtcNow;
    public DateTime release_updated_at { get; set; } = DateTime.UtcNow;
    public Guid created_by_user_id { get; set; }
    public Guid? published_by_user_id { get; set; }

    public User? created_by_user { get; set; }
    public User? published_by_user { get; set; }

    public static Release Build(Guid createdByUserId, string? version, string title, string summary, string body)
    {
        Validate(title, summary, body);

        return new Release
        {
            created_by_user_id = createdByUserId,
            release_version = NormalizeVersion(version),
            release_title = title.Trim(),
            release_summary = summary.Trim(),
            release_body = body.Trim()
        };
    }

    public void Update(string? version, string title, string summary, string body)
    {
        if (release_status == "published")
        {
            throw new InvalidOperationException("Published releases cannot be edited.");
        }

        Validate(title, summary, body);

        release_version = NormalizeVersion(version);
        release_title = title.Trim();
        release_summary = summary.Trim();
        release_body = body.Trim();
    }

    public void Publish(Guid publishedByUserId, DateTime publishedAtUtc)
    {
        if (release_status == "published")
        {
            throw new InvalidOperationException("Release is already published.");
        }

        release_status = "published";
        release_published_at = publishedAtUtc;
        published_by_user_id = publishedByUserId;
    }

    private static void Validate(string title, string summary, string body)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new InvalidOperationException("Release title is required.");
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new InvalidOperationException("Release summary is required.");
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new InvalidOperationException("Release body is required.");
        }
    }

    private static string? NormalizeVersion(string? version)
    {
        return string.IsNullOrWhiteSpace(version) ? null : version.Trim();
    }
}
