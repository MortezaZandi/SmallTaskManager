using SmallTask.Models;

namespace SmallTask.Services;

public interface IAttachmentService
{
    Task<Attachment?> GetByIdAsync(int attachmentId);
    Task<IReadOnlyList<Attachment>> GetByTaskIdAsync(int taskId);
    Task<Attachment> AddAsync(int taskId, string originalFileName, Stream fileContent);
    Task DeleteAsync(int attachmentId);
    string GetPhysicalFilePath(Attachment attachment);
}
