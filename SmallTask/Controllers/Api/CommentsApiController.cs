using Microsoft.AspNetCore.Mvc;
using SmallTask.Services;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/comments")]
public class CommentsApiController : ControllerBase
{
    private readonly ICommentService _service;

    public CommentsApiController(ICommentService service) => _service = service;

    [HttpGet("task/{taskId:int}")]
    public async Task<IActionResult> GetByTask(int taskId) =>
        Ok(await _service.GetByTaskIdAsync(taskId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var c = await _service.GetByIdAsync(id);
        return c == null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest req)
    {
        try
        {
            var c = await _service.AddAsync(req.TaskId, req.UserId, req.Text);
            return CreatedAtAction(nameof(Get), new { id = c.CommentId }, c);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCommentRequest req)
    {
        try
        {
            await _service.UpdateAsync(id, req.Text);
            return Ok();
        }
        catch (InvalidOperationException)
        {
            return NotFound();
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

public class CreateCommentRequest
{
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = "";
}

public class UpdateCommentRequest
{
    public string Text { get; set; } = "";
}
