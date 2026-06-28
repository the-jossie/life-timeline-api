public class MilestoneQuery
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public int? Year { get; set; }

    public string? Mood { get; set; }

    public string? Tag { get; set; }

    public string? Search { get; set; }
}
