using LifeTimelineApi.Entities;

namespace LifeTimelineApi.Application.Services.Milestones;

public interface IMilestoneService
{
    Task<Milestone> CreateAsync(CreateMilestoneRequest request);
    Task<Milestone?> GetByIdAsync(Guid id);
    Task<List<Milestone>> GetAllAsync();
    Task<bool> UpdateAsync(Guid id, UpdateMilestoneRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<MilestoneStatsDto> GetStatsAsync();
}
