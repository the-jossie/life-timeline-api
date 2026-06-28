using LifeTimelineApi.Application.Services.Milestones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("api/milestones")]
public class MilestonesController : ControllerBase
{
    private readonly IMilestoneService _service;

    public MilestonesController(IMilestoneService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateMilestoneRequest request)
    {
        var milestone = await _service.CreateAsync(request);

        return Ok(new { milestone, Message = "Milestone created successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
    [FromQuery] MilestoneQuery query, CancellationToken cancellationToken)
    {
        var milestones = await _service.GetAllAsync(query, cancellationToken);

        return Ok(milestones);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var milestone = await _service.GetByIdAsync(id, cancellationToken);

        if (milestone == null)
        {
            return NotFound(new { message = "Milestone not found." });
        }

        return Ok(milestone);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMilestoneRequest request, CancellationToken cancellationToken)
    {
        var milestone = await _service.UpdateAsync(id, request, cancellationToken);

        if (milestone == null)
        {
            return NotFound(new { message = "Milestone not found." });
        }

        return Ok(new { milestone, Message = "Milestone updated successfully." });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var milestone = await _service.GetByIdAsync(id, cancellationToken);

        if (milestone == null)
        {
            return NotFound(new { message = "Milestone not found." });
        }

        await _service.DeleteAsync(id);

        return Ok(new { Message = "Milestone deleted successfully." });
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken cancellationToken)
    {
        var stats = await _service.GetStatsAsync(cancellationToken);

        return Ok(stats);
    }
}
