using LifeTimelineApi.Entities;

public interface IMilestoneService
{
    Task<Milestone> CreateAsync(CreateMilestoneRequest request);
    Task<Milestone?> GetMilestoneByIdAsync(Guid id);
    Task<IEnumerable<Milestone>> GetAllMilestonesAsync();
    Task<Milestone> UpdateMilestoneAsync(UpdateMilestoneRequest request);
    Task<bool> DeleteMilestoneAsync(Guid id);
}
