using SmallTask.Models;

namespace SmallTask.Repositories;

public interface IAttachmentRepository
{
    Task<Attachment?> GetByIdAsync(int attachmentId, bool includeDeleted = false);
    Task<IReadOnlyList<Attachment>> GetByTaskIdAsync(int taskId, bool includeDeleted = false);
    Task<Attachment> AddAsync(Attachment attachment);
    Task UpdateAsync(Attachment attachment);
}
