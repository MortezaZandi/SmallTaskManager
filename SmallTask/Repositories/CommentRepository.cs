using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;

namespace SmallTask.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDbContext _db;

    public CommentRepository(AppDbContext db) => _db = db;

    public async Task<Comment?> GetByIdAsync(int commentId, bool includeDeleted = false)
    {
        var q = _db.Comments.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.CommentId == commentId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Comment>> GetByTaskIdAsync(int taskId, bool includeDeleted = false)
    {
        var q = _db.Comments.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.TaskId == taskId)
            .OrderBy(x => x.CreatedAt);
        if (!includeDeleted) q = (IOrderedQueryable<Comment>)q.Where(x => !x.IsDeleted);
        return await q.ToListAsync();
    }

    public async Task<Comment> AddAsync(Comment comment)
    {
        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return comment;
    }

    public async Task UpdateAsync(Comment comment)
    {
        _db.Comments.Update(comment);
        await _db.SaveChangesAsync();
    }
}
