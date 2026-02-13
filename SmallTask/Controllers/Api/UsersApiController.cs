using Microsoft.AspNetCore.Mvc;
using SmallTask.Services;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/users")]
public class UsersApiController : ControllerBase
{
    private readonly IUserService _service;

    public UsersApiController(IUserService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpGet("with-task-counts")]
    public async Task<IActionResult> GetWithTaskCounts() => Ok(await _service.GetUsersWithTaskCountsAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var u = await _service.GetByIdAsync(id);
        return u == null ? NotFound() : Ok(u);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
    {
        try
        {
            var u = await _service.CreateAsync(req.Name, req.IconPath);
            return CreatedAtAction(nameof(Get), new { id = u.UserId }, u);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest req)
    {
        try
        {
            await _service.UpdateAsync(id, req.Name, req.IconPath);
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

public class CreateUserRequest
{
    public string Name { get; set; } = "";
    public string? IconPath { get; set; }
}

public class UpdateUserRequest
{
    public string Name { get; set; } = "";
    public string? IconPath { get; set; }
}
