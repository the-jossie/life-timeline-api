public class MilestoneDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Emoji { get; set; } = "";
    public string Mood { get; set; } = "";
    public DateTime Date { get; set; }
    public List<string> Tags { get; set; } = new();
}
