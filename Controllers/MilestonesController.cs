using LifeTimelineApi.Data;
using LifeTimelineApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/milestones")]
public class MilestonesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public MilestonesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateMilestoneRequest request)
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

        return Ok(new { milestone, Message = "Milestone created successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var milestones = await _dbContext.Milestones.OrderByDescending(m => m.Date).ToListAsync();

        return Ok(milestones);
    }
}
