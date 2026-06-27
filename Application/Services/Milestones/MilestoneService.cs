using LifeTimelineApi.Data;
using LifeTimelineApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeTimelineApi.Application.Services.Milestones;

public class MilestoneService : IMilestoneService
{
    private readonly AppDbContext _dbContext;

    public MilestoneService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Milestone> CreateAsync(CreateMilestoneRequest request)
    {
        var milestone = new Milestone
        {
            Title = request.Title,
            Description = request.Description,
            Emoji = request.Emoji,
            Mood = request.Mood,
            Date = request.Date
        };

        _dbContext.Milestones.Add(milestone);
        await _dbContext.SaveChangesAsync();

        return milestone;
    }

    public async Task<List<Milestone>> GetAllAsync()
    {
        return await _dbContext.Milestones.OrderByDescending(m => m.Date).ToListAsync();
    }

    public async Task<Milestone?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Milestones.FindAsync(id);
    }


    public async Task<Milestone> UpdateAsync(UpdateMilestoneRequest request)
    {
        var milestone = await _dbContext.Milestones.FindAsync(request.Id);

        if (milestone == null)
        {
            throw new KeyNotFoundException("Milestone not found.");
        }

        milestone.Title = request.Title;
        milestone.Description = request.Description;
        milestone.Emoji = request.Emoji;
        milestone.Mood = request.Mood;
        milestone.Date = request.Date;

        await _dbContext.SaveChangesAsync();

        return milestone;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var milestone = await _dbContext.Milestones.FindAsync(id);

        if (milestone == null)
        {
            return false;
        }

        _dbContext.Milestones.Remove(milestone);
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
