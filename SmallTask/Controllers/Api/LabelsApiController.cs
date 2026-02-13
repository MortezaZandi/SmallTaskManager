using Microsoft.AspNetCore.Mvc;
using SmallTask.Services;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/labels")]
public class LabelsApiController : ControllerBase
{
    private readonly ILabelService _service;

    public LabelsApiController(ILabelService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int projectId) => Ok(await _service.GetAllAsync(projectId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var l = await _service.GetByIdAsync(id);
        return l == null ? NotFound() : Ok(l);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLabelRequest req)
    {
        try
        {
            var l = await _service.CreateAsync(req.ProjectId, req.Name, req.Description, req.Color ?? "#000000");
            return CreatedAtAction(nameof(Get), new { id = l.LabelId }, l);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateLabelRequest req)
    {
        try
        {
            await _service.UpdateAsync(id, req.Name, req.Description, req.Color ?? "#000000");
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

public class CreateLabelRequest
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Color { get; set; }
}

public class UpdateLabelRequest
{
    public string Name { get; set; } = "";
    public string? Description { get; set; }
    public string? Color { get; set; }
}
