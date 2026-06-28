public class Tag
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public ICollection<MilestoneTag> MilestoneTags { get; set; }
        = new List<MilestoneTag>();
}
