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
        return await _dbContext.Milestones.Where(m => m.Id == id)
        .Select(m => new Milestone
        {
            Id = m.Id,
            Title = m.Title,
            Description = m.Description,
            Emoji = m.Emoji,
            Mood = m.Mood,
            Date = m.Date
        })
        .FirstOrDefaultAsync();
    }


    public async Task<bool> UpdateAsync(Guid id, UpdateMilestoneRequest request)
    {
        var milestone = await _dbContext.Milestones.FirstOrDefaultAsync(m => m.Id == id);

        if (milestone == null)
        {
            return false;
        }

        milestone.Title = request.Title;
        milestone.Description = request.Description;
        milestone.Emoji = request.Emoji;
        milestone.Mood = request.Mood;
        milestone.Date = request.Date;

        await _dbContext.SaveChangesAsync();

        return true;
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

    public async Task<MilestoneStatsDto> GetStatsAsync()
    {
        var totalMilestones = await _dbContext.Milestones.CountAsync();
        var totalMilestonesThisMonth = await _dbContext.Milestones.CountAsync(m => m.Date.Month == DateTime.Now.Month && m.Date.Year == DateTime.Now.Year);
        var totalMilestonesThisYear = await _dbContext.Milestones.CountAsync(m => m.Date.Year == DateTime.Now.Year);

        return new MilestoneStatsDto
        {
            TotalMilestones = totalMilestones,
            TotalMilestonesThisMonth = totalMilestonesThisMonth,
            TotalMilestonesThisYear = totalMilestonesThisYear
        };
    }
}
