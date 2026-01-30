using SmallTask.Models;

namespace SmallTask.Repositories;

public interface ICommentRepository
{
    Task<Comment?> GetByIdAsync(int commentId, bool includeDeleted = false);
    Task<IReadOnlyList<Comment>> GetByTaskIdAsync(int taskId, bool includeDeleted = false);
    Task<Comment> AddAsync(Comment comment);
    Task UpdateAsync(Comment comment);
}
