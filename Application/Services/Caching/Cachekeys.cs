public static class CacheKeys
{
    public static string Milestones(Guid userId, string version, MilestoneQuery query)
    {
        return $"milestones:{version}:{userId}:{query.Year}:{query.Mood}:{query.Tag}:{query.Search}:{query.Page}:{query.PageSize}";
    }

    public static string MilestonesVersion(Guid userId)
    {
        return $"milestones:version:{userId}";
    }
}
