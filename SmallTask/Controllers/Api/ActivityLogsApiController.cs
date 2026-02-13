using Microsoft.AspNetCore.Mvc;
using SmallTask.Services;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/activity-logs")]
public class ActivityLogsApiController : ControllerBase
{
    private readonly IActivityLogService _service;

    public ActivityLogsApiController(IActivityLogService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int? projectId,
        [FromQuery] int? userId,
        [FromQuery] int? taskId,
        [FromQuery] int? taskNumber,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        var (items, total) = await _service.GetPagedAsync(projectId, userId, taskId, taskNumber, page, Math.Min(pageSize, 100));
        return Ok(new { items, total });
    }

    [HttpGet("last-by-task/{taskId:int}")]
    public async Task<IActionResult> GetLastByTask(int taskId)
    {
        var log = await _service.GetLastByTaskIdAsync(taskId);
        return log == null ? NotFound() : Ok(log);
    }

    [HttpPost("last-by-tasks")]
    public async Task<IActionResult> GetLastByTasks([FromBody] int[] taskIds)
    {
        if (taskIds == null || taskIds.Length == 0) return Ok(new Dictionary<int, object>());
        var result = await _service.GetLastByTaskIdsAsync(taskIds);
        return Ok(result);
    }
}
