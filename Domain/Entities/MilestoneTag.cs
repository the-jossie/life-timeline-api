using LifeTimelineApi.Entities;

public class MilestoneTag
{
    public Guid MilestoneId { get; set; }
    public Milestone Milestone { get; set; } = null!;

    public Guid TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
