namespace LifeTimelineApi.Entities;

public class Milestone
{
    public Guid Id { get; set; }

    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public string Emoji { get; set; } = "";

    public string Mood { get; set; } = "";

    public DateTime Date { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;
}
