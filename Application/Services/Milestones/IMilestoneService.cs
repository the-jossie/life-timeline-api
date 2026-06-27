using LifeTimelineApi.Entities;

namespace LifeTimelineApi.Application.Services.Milestones;

public interface IMilestoneService
{
    Task<Milestone> CreateAsync(CreateMilestoneRequest request);
    Task<Milestone?> GetByIdAsync(Guid id);
    Task<List<Milestone>> GetAllAsync();
    Task<Milestone> UpdateAsync(UpdateMilestoneRequest request);
    Task<bool> DeleteAsync(Guid id);
}
