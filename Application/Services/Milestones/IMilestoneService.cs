namespace LifeTimelineApi.Application.Services.Milestones;

public interface IMilestoneService
{
    Task<MilestoneDto> CreateAsync(CreateMilestoneRequest request);
    Task<MilestoneDto?> GetByIdAsync(Guid id);
    Task<List<MilestoneDto>> GetAllAsync();
    Task<MilestoneDto?> UpdateAsync(Guid id, UpdateMilestoneRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<MilestoneStatsDto> GetStatsAsync();
}
