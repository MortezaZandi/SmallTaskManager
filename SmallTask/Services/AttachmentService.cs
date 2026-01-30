using Microsoft.AspNetCore.Hosting;
using SmallTask.Models;
using SmallTask.Repositories;
using SmallTask.Settings;

namespace SmallTask.Services;

public class AttachmentService : IAttachmentService
{
    private readonly IAttachmentRepository _repo;
    private readonly AppSettings _settings;
    private readonly IWebHostEnvironment _env;

    public AttachmentService(IAttachmentRepository repo, Microsoft.Extensions.Options.IOptions<AppSettings> options, IWebHostEnvironment env)
    {
        _repo = repo;
        _settings = options.Value;
        _env = env;
    }

    private string GetStorageRoot() =>
        Path.IsPathRooted(_settings.AttachmentStoragePath)
            ? _settings.AttachmentStoragePath
            : Path.Combine(_env.ContentRootPath, _settings.AttachmentStoragePath);

    public async Task<Attachment?> GetByIdAsync(int attachmentId) => await _repo.GetByIdAsync(attachmentId);

    public async Task<IReadOnlyList<Attachment>> GetByTaskIdAsync(int taskId) => await _repo.GetByTaskIdAsync(taskId);

    public async Task<Attachment> AddAsync(int taskId, string originalFileName, Stream fileContent)
    {
        var dir = GetStorageRoot();
        Directory.CreateDirectory(dir);
        var ext = Path.GetExtension(originalFileName);
        var storedName = $"{Guid.NewGuid():N}{ext}";
        var filePath = Path.Combine(dir, storedName);
        await using (var fs = File.Create(filePath))
            await fileContent.CopyToAsync(fs);
        var attachment = new Attachment
        {
            TaskId = taskId,
            OriginalFileName = originalFileName,
            StoredFileName = storedName,
            FilePath = filePath,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
        return await _repo.AddAsync(attachment);
    }

    public async Task DeleteAsync(int attachmentId)
    {
        var a = await _repo.GetByIdAsync(attachmentId, includeDeleted: true);
        if (a == null) throw new InvalidOperationException("Attachment not found.");
        var path = GetPhysicalFilePath(a);
        if (File.Exists(path)) File.Delete(path);
        var att = new Attachment
        {
            AttachmentId = a.AttachmentId,
            TaskId = a.TaskId,
            OriginalFileName = a.OriginalFileName,
            StoredFileName = a.StoredFileName,
            FilePath = a.FilePath,
            CreatedAt = a.CreatedAt,
            IsDeleted = true
        };
        await _repo.UpdateAsync(att);
    }

    public string GetPhysicalFilePath(Attachment attachment) =>
        Path.Combine(GetStorageRoot(), attachment.StoredFileName);
}