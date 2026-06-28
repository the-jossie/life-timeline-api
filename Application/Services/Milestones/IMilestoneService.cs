namespace LifeTimelineApi.Application.Services.Milestones;

public interface IMilestoneService
{
    Task<MilestoneDto> CreateAsync(CreateMilestoneRequest request);
    Task<MilestoneDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<MilestoneDto>> GetAllAsync(MilestoneQuery query, CancellationToken cancellationToken);
    Task<MilestoneDto?> UpdateAsync(Guid id, UpdateMilestoneRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id);
    Task<MilestoneStatsDto> GetStatsAsync(CancellationToken cancellationToken);
}
