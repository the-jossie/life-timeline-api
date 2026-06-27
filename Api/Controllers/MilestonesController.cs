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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var milestone = await _dbContext.Milestones.FindAsync(id);

        if (milestone == null)
        {
            return NotFound(new { message = "Milestone not found." });
        }

        return Ok(milestone);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMilestoneRequest request)
    {
        var milestone = await _dbContext.Milestones.FindAsync(id);

        if (milestone == null)
        {
            return NotFound(new { message = "Milestone not found." });
        }

        milestone.Title = request.Title;
        milestone.Description = request.Description;
        milestone.Emoji = request.Emoji;
        milestone.Mood = request.Mood;
        milestone.Date = request.Date;

        await _dbContext.SaveChangesAsync();

        return Ok(new { milestone, Message = "Milestone updated successfully." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var milestone = await _dbContext.Milestones.FindAsync(id);

        if (milestone == null)
        {
            return NotFound(new { message = "Milestone not found." });
        }

        _dbContext.Milestones.Remove(milestone);
        await _dbContext.SaveChangesAsync();

        return Ok(new { Message = "Milestone deleted successfully." });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalMilestones = await _dbContext.Milestones.CountAsync();
        var totalMilestonesThisMonth = await _dbContext.Milestones.CountAsync(m => m.Date.Month == DateTime.Now.Month && m.Date.Year == DateTime.Now.Year);
        var totalMilestonesThisYear = await _dbContext.Milestones.CountAsync(m => m.Date.Year == DateTime.Now.Year);

        return Ok(new
        {
            totalMilestones,
            totalMilestonesThisMonth,
            totalMilestonesThisYear
        });
    }
}
