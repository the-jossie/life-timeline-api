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
            Id = Guid.NewGuid(),
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

    public async Task<PagedResult<MilestoneDto>> GetAllAsync(MilestoneQuery query, CancellationToken cancellationToken)
    {
        var baseQuery = _dbContext.Milestones
        .AsNoTracking()
        .Where(m => m.UserId == _currentUser.UserId)
        .AsQueryable();

        if (query.Year.HasValue)
        {
            baseQuery = baseQuery.Where(m => m.Date.Year == query.Year);
        }
        if (!string.IsNullOrEmpty(query.Mood))
        {
            baseQuery = baseQuery.Where(m => m.Mood == query.Mood);
        }
        if (!string.IsNullOrEmpty(query.Tag))
        {
            baseQuery = baseQuery
                .Where(m => m.MilestoneTags
                    .Any(t => t.Tag.Name == query.Tag)
                );
        }
        if (!string.IsNullOrEmpty(query.Search))
        {
            baseQuery = baseQuery
                .Where(m => m.Title
                    .Contains(query.Search) || m.Description.Contains(query.Search)
                );
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken: cancellationToken);

        var items = await baseQuery
            .OrderByDescending(m => m.Date)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Include(m => m.MilestoneTags)
            .ThenInclude(mt => mt.Tag)
            .Select(m => ToMilestoneDto(m))
            .ToListAsync(cancellationToken);

        return new PagedResult<MilestoneDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    public async Task<MilestoneDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbContext.Milestones
            .AsNoTracking()
            .Where(m => m.Id == id && m.UserId == _currentUser.UserId)
            .Select(m => ToMilestoneDto(m))
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<MilestoneDto?> UpdateAsync(Guid id, UpdateMilestoneRequest request, CancellationToken cancellationToken)
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

        await _dbContext.SaveChangesAsync(cancellationToken);

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

    public async Task<MilestoneStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var milestones = _dbContext.Milestones
            .AsNoTracking()
            .Where(m => m.UserId == _currentUser.UserId);

        var total = await milestones.CountAsync(cancellationToken);
        var thisMonth = await milestones.CountAsync(m => m.Date.Month == now.Month && m.Date.Year == now.Year, cancellationToken);
        var thisYear = await milestones.CountAsync(m => m.Date.Year == now.Year, cancellationToken);

        return new MilestoneStatsDto
        {
            Total = total,
            ThisMonth = thisMonth,
            ThisYear = thisYear
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
