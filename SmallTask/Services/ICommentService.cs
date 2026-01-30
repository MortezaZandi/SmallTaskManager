using SmallTask.Models;

namespace SmallTask.Services;

public interface ICommentService
{
    Task<Comment?> GetByIdAsync(int commentId);
    Task<IReadOnlyList<Comment>> GetByTaskIdAsync(int taskId);
    Task<Comment> AddAsync(int taskId, int userId, string text);
    Task UpdateAsync(int commentId, string text);
    Task DeleteAsync(int commentId);
}
