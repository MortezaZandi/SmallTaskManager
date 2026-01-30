using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;

namespace SmallTask.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly AppDbContext _db;

    public AttachmentRepository(AppDbContext db) => _db = db;

    public async Task<Attachment?> GetByIdAsync(int attachmentId, bool includeDeleted = false)
    {
        var q = _db.Attachments.AsNoTracking().Where(x => x.AttachmentId == attachmentId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Attachment>> GetByTaskIdAsync(int taskId, bool includeDeleted = false)
    {
        var q = _db.Attachments.AsNoTracking()
            .Where(x => x.TaskId == taskId)
            .OrderBy(x => x.CreatedAt);
        if (!includeDeleted) q = (IOrderedQueryable<Attachment>)q.Where(x => !x.IsDeleted);
        return await q.ToListAsync();
    }

    public async Task<Attachment> AddAsync(Attachment attachment)
    {
        _db.Attachments.Add(attachment);
        await _db.SaveChangesAsync();
        return attachment;
    }

    public async Task UpdateAsync(Attachment attachment)
    {
        _db.Attachments.Update(attachment);
        await _db.SaveChangesAsync();
    }
}
