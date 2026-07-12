namespace LifeTimelineApi.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }

    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAt;

    public bool IsRevoked =>
        RevokedAt != null;

    public bool IsActive =>
        !IsExpired && !IsRevoked;
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
