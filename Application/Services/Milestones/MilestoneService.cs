using LifeTimelineApi.Data;
using LifeTimelineApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace LifeTimelineApi.Application.Services.Milestones;

public class MilestoneService : IMilestoneService
{
    private readonly AppDbContext _dbContext;
    private readonly CurrentUserService _currentUser;
    private readonly CacheService _cacheService;

    public MilestoneService(AppDbContext dbContext, CurrentUserService currentUser, CacheService cache)
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
        _cacheService = cache;
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

        await _cacheService.SetAsync(
            CacheKeys.MilestonesVersion(_currentUser.UserId),
            Guid.NewGuid().ToString(),
            TimeSpan.FromDays(30)
        );

        return ToMilestoneDto(milestone);
    }

    public async Task<PagedResult<MilestoneDto>> GetAllAsync(MilestoneQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;

        var cacheVersionKey = CacheKeys.MilestonesVersion(userId);
        var cacheVersion = await _cacheService.GetAsync<string>(cacheVersionKey) ?? "1";
        var cacheKey = CacheKeys.Milestones(userId, cacheVersion, query);

        var cachedResult = await _cacheService.GetAsync<PagedResult<MilestoneDto>>(cacheKey);

        if (cachedResult != null)
        {
            return cachedResult;
        }

        var baseQuery = _dbContext.Milestones
            .AsNoTracking()
            .Where(m => m.UserId == userId);

        if (query.Year.HasValue)
        {
            baseQuery = baseQuery.Where(m => m.Date.Year == query.Year.Value);
        }
        if (!string.IsNullOrWhiteSpace(query.Mood))
        {
            baseQuery = baseQuery.Where(m => m.Mood == query.Mood);
        }
        if (!string.IsNullOrWhiteSpace(query.Tag))
        {
            baseQuery = baseQuery
                .Where(m => m.MilestoneTags
                    .Any(t => t.Tag.Name == query.Tag)
                );
        }
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            baseQuery = baseQuery
                .Where(m => m.Title
                    .Contains(search) || m.Description.Contains(search)
                );
        }

        var totalCount = await baseQuery.CountAsync(cancellationToken: cancellationToken);

        var data = await baseQuery
            .OrderByDescending(m => m.Date)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Include(m => m.MilestoneTags)
            .ThenInclude(mt => mt.Tag)
            .ToListAsync(cancellationToken);

        var items = data.Select(ToMilestoneDto).ToList();

        var result = new PagedResult<MilestoneDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        await _cacheService.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));

        return result;
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

        await _cacheService.SetAsync(
            CacheKeys.MilestonesVersion(_currentUser.UserId),
            Guid.NewGuid().ToString(),
            TimeSpan.FromDays(30)
        );

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

        await _cacheService.SetAsync(
            CacheKeys.MilestonesVersion(_currentUser.UserId),
            Guid.NewGuid().ToString(),
            TimeSpan.FromDays(30)
        );

        return true;
    }

    public async Task<MilestoneStatsDto> GetStatsAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await _dbContext.Milestones
        .AsNoTracking()
        .Where(m => m.UserId == _currentUser.UserId)
        .GroupBy(_ => 1)
        .Select(g => new MilestoneStatsDto
        {
            Total = g.Count(),

            ThisMonth = g.Count(m =>
                m.Date >= new DateTime(now.Year, now.Month, 1) &&
                m.Date < new DateTime(now.Year, now.Month, 1).AddMonths(1)
            ),

            ThisYear = g.Count(m =>
                m.Date >= new DateTime(now.Year, 1, 1) &&
                m.Date < new DateTime(now.Year + 1, 1, 1)
            )
        })
        .FirstOrDefaultAsync(cancellationToken)
        ?? new MilestoneStatsDto();

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
