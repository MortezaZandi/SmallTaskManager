using Microsoft.AspNetCore.Mvc;
using SmallTask.Services;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/projects")]
public class ProjectsApiController : ControllerBase
{
    private readonly IProjectService _service;

    public ProjectsApiController(IProjectService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await _service.GetAllWithTaskCountAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _service.GetByIdAsync(id);
        return p == null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest req)
    {
        try
        {
            var p = await _service.CreateAsync(req.Name, req.Description);
            return CreatedAtAction(nameof(Get), new { id = p.ProjectId }, p);
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
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class CreateProjectRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
}
