using Microsoft.AspNetCore.Mvc;
using SmallTask.Services;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/attachments")]
public class AttachmentsApiController : ControllerBase
{
    private readonly IAttachmentService _service;

    public AttachmentsApiController(IAttachmentService service) => _service = service;

    [HttpGet("task/{taskId:int}")]
    public async Task<IActionResult> GetByTask(int taskId) =>
        Ok(await _service.GetByTaskIdAsync(taskId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var a = await _service.GetByIdAsync(id);
        return a == null ? NotFound() : Ok(a);
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var a = await _service.GetByIdAsync(id);
        if (a == null) return NotFound();
        var path = _service.GetPhysicalFilePath(a);
        if (!System.IO.File.Exists(path)) return NotFound();
        var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, "application/octet-stream", a.OriginalFileName);
    }

    [HttpPost]
    public async Task<IActionResult> Upload([FromForm] int taskId, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file.");
        try
        {
            await using var stream = file.OpenReadStream();
            var a = await _service.AddAsync(taskId, file.FileName, stream);
            return CreatedAtAction(nameof(Get), new { id = a.AttachmentId }, a);
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
