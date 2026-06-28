using LifeTimelineApi.Data;
using LifeTimelineApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeTimelineApi.Application.Services.Milestones;

public class MilestoneService : IMilestoneService
{
    private readonly AppDbContext _dbContext;
    private readonly CurrentUserService _currentUser;
    public MilestoneService(AppDbContext dbContext, CurrentUserService currentUser)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    public async Task<MilestoneDto> CreateAsync(CreateMilestoneRequest request)
    {
        var milestone = new Milestone
        {
            Title = request.Title,
            Description = request.Description,
            Emoji = request.Emoji,
            Mood = request.Mood,
            Date = request.Date,
            UserId = _currentUser.UserId
        };

        _dbContext.Milestones.Add(milestone);
        await _dbContext.SaveChangesAsync();

        return ToMilestoneDto(milestone);
    }

    public async Task<List<MilestoneDto>> GetAllAsync()
    {
        return await _dbContext.Milestones
        .AsNoTracking()
        .Where(m => m.UserId == _currentUser.UserId)
        .OrderByDescending(m => m.Date)
        .Select(m => ToMilestoneDto(m))
        .ToListAsync();
    }

    public async Task<MilestoneDto?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Milestones
            .AsNoTracking()
            .Where(m => m.Id == id && m.UserId == _currentUser.UserId)
            .Select(m => ToMilestoneDto(m))
            .FirstOrDefaultAsync();
    }


    public async Task<MilestoneDto?> UpdateAsync(Guid id, UpdateMilestoneRequest request)
    {
        var milestone = await _dbContext.Milestones
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);

        if (milestone == null)
        {
            return null;
        }

        milestone.Title = request.Title;
        milestone.Description = request.Description;
        milestone.Emoji = request.Emoji;
        milestone.Mood = request.Mood;
        milestone.Date = request.Date;

        await _dbContext.SaveChangesAsync();

        return ToMilestoneDto(milestone);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var milestone = await _dbContext.Milestones
            .FirstOrDefaultAsync(m => m.Id == id && m.UserId == _currentUser.UserId);

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
        var totalMilestones = await _dbContext.Milestones
        .AsNoTracking()
            .Where(m =>
                m.UserId == _currentUser.UserId)
            .CountAsync();
        var totalMilestonesThisMonth = await _dbContext.Milestones
            .AsNoTracking()
            .Where(m => m.UserId == _currentUser.UserId)
            .CountAsync(m => m.Date.Month == DateTime.Now.Month && m.Date.Year == DateTime.Now.Year);
        var totalMilestonesThisYear = await _dbContext.Milestones
            .AsNoTracking()
            .Where(m => m.UserId == _currentUser.UserId)
            .CountAsync(m => m.Date.Year == DateTime.Now.Year);

        return new MilestoneStatsDto
        {
            TotalMilestones = totalMilestones,
            TotalMilestonesThisMonth = totalMilestonesThisMonth,
            TotalMilestonesThisYear = totalMilestonesThisYear
        };
    }

    private static MilestoneDto ToMilestoneDto(Milestone milestone)
    {
        return new MilestoneDto
        {
            Id = milestone.Id,
            Title = milestone.Title,
            Description = milestone.Description,
            Emoji = milestone.Emoji,
            Mood = milestone.Mood,
            Date = milestone.Date
        };
    }
}
