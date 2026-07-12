public class UpdateMilestoneRequest
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Emoji { get; set; } = "";
    public string Mood { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public DateTime Date { get; set; }
}
