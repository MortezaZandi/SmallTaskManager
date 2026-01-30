using Microsoft.AspNetCore.Mvc;
using SmallTask.Models;
using SmallTask.Repositories;
using SmallTask.Services;
using TaskStatus = SmallTask.Models.TaskStatus;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/tasks")]
public class TasksApiController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksApiController(ITaskService service) => _service = service;

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var t = await _service.GetByIdAsync(id);
        return t == null ? NotFound() : Ok(t);
    }

    [HttpGet("filtered")]
    public async Task<IActionResult> GetFiltered([FromQuery] TaskFilterQuery q)
    {
        var filter = new TaskFilter
        {
            Text = q.Text,
            GroupId = q.GroupId,
            LabelId = q.LabelId,
            Priority = q.Priority,
            Status = q.Status,
            AssignedUserId = q.AssignedUserId,
            TaskNumber = q.TaskNumber
        };
        var list = await _service.GetFilteredAsync(filter);
        return Ok(list);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest req)
    {
        try
        {
            var t = await _service.CreateAsync(
                req.Title,
                req.Description,
                req.Status,
                req.Priority,
                req.AssignedUserId,
                req.GroupId,
                req.LabelIds);
            return CreatedAtAction(nameof(Get), new { id = t.TaskId }, t);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskRequest req)
    {
        try
        {
            await _service.UpdateAsync(
                id,
                req.Title,
                req.Description,
                req.Status,
                req.Priority,
                req.AssignedUserId,
                req.GroupId,
                req.LabelIds);
            return Ok();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }
    }
}

public class TaskFilterQuery
{
    public string? Text { get; set; }
    public int? GroupId { get; set; }
    public int? LabelId { get; set; }
    public TaskPriority? Priority { get; set; }
    public TaskStatus? Status { get; set; }
    public int? AssignedUserId { get; set; }
    public int? TaskNumber { get; set; }
}

public class CreateTaskRequest
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int? AssignedUserId { get; set; }
    public int? GroupId { get; set; }
    public IReadOnlyList<int>? LabelIds { get; set; }
}

public class UpdateTaskRequest
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int? AssignedUserId { get; set; }
    public int? GroupId { get; set; }
    public IReadOnlyList<int>? LabelIds { get; set; }
}
