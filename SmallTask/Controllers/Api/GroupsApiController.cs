using Microsoft.AspNetCore.Mvc;
using SmallTask.Models;
using SmallTask.Services;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/groups")]
public class GroupsApiController : ControllerBase
{
    private readonly IGroupService _service;

    public GroupsApiController(IGroupService service) => _service = service;

    [HttpGet("roots")]
    public async Task<IActionResult> GetRoots() =>
        Ok(await _service.GetRootGroupsAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var g = await _service.GetByIdAsync(id);
        return g == null ? NotFound() : Ok(g);
    }

    [HttpGet("{id:int}/children")]
    public async Task<IActionResult> GetChildren(int id) =>
        Ok(await _service.GetChildrenAsync(id));

    [HttpGet("flat")]
    public async Task<IActionResult> GetAllFlat() =>
        Ok(await _service.GetAllFlatAsync());

    [HttpGet("with-task-count")]
    public async Task<IActionResult> GetWithTaskCount([FromQuery] int? projectId) =>
        Ok(await _service.GetAllWithTaskCountAsync(projectId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest req)
    {
        try
        {
            var g = await _service.CreateAsync(req.Name, req.ParentGroupId);
            return CreatedAtAction(nameof(Get), new { id = g.GroupId }, g);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/rename")]
    public async Task<IActionResult> Rename(int id, [FromBody] RenameGroupRequest req)
    {
        try
        {
            await _service.RenameAsync(id, req.Name);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}/move")]
    public async Task<IActionResult> Move(int id, [FromBody] MoveGroupRequest req)
    {
        try
        {
            await _service.MoveAsync(id, req.NewParentGroupId);
            return Ok();
        }
        catch (InvalidOperationException ex)
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

public class CreateGroupRequest
{
    public string Name { get; set; } = "";
    public int? ParentGroupId { get; set; }
}

public class RenameGroupRequest
{
    public string Name { get; set; } = "";
}

public class MoveGroupRequest
{
    public int? NewParentGroupId { get; set; }
}
