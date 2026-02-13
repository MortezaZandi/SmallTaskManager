using Microsoft.AspNetCore.Mvc;

namespace SmallTask.Controllers.Api;

[ApiController]
[Route("api/images")]
public class ImagesApiController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private static readonly string[] AllowedExtensions = { ".png", ".jpg", ".jpeg", ".gif", ".webp" };

    public ImagesApiController(IWebHostEnvironment env) => _env = env;

    /// <summary>Lists image paths in wwwroot/images (including avatars subfolder).</summary>
    [HttpGet("avatars")]
    public IActionResult ListAvatars()
    {
        var imagesPath = Path.Combine(_env.WebRootPath, "images");
        if (!Directory.Exists(imagesPath)) return Ok(Array.Empty<string>());

        var paths = new List<string>();
        ScanImages(imagesPath, _env.WebRootPath, paths);
        paths.Sort(StringComparer.OrdinalIgnoreCase);
        return Ok(paths);
    }

    private void ScanImages(string physicalDir, string webRoot, List<string> paths)
    {
        foreach (var file in Directory.EnumerateFiles(physicalDir))
        {
            var ext = Path.GetExtension(file);
            if (AllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            {
                var rel = Path.GetRelativePath(webRoot, file).Replace('\\', '/');
                paths.Add("/" + rel);
            }
        }
        foreach (var sub in Directory.EnumerateDirectories(physicalDir))
            ScanImages(sub, webRoot, paths);
    }

    /// <summary>Uploads an image to wwwroot/images/avatars. Returns the URL path.</summary>
    [HttpPost("avatars")]
    public async Task<IActionResult> UploadAvatar([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest(new { message = "No file." });
        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
            return BadRequest(new { message = "Invalid file type. Allowed: png, jpg, jpeg, gif, webp." });

        var avatarsDir = Path.Combine(_env.WebRootPath, "images", "avatars");
        Directory.CreateDirectory(avatarsDir);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var physicalPath = Path.Combine(avatarsDir, fileName);
        await using (var stream = new FileStream(physicalPath, FileMode.Create))
            await file.CopyToAsync(stream);

        var urlPath = "/images/avatars/" + fileName;
        return Ok(new { path = urlPath });
    }
}
