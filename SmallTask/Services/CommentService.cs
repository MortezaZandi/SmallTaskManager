using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _repo;

    public CommentService(ICommentRepository repo) => _repo = repo;

    public async Task<Comment?> GetByIdAsync(int commentId) => await _repo.GetByIdAsync(commentId);

    public async Task<IReadOnlyList<Comment>> GetByTaskIdAsync(int taskId) => await _repo.GetByTaskIdAsync(taskId);

    public async Task<Comment> AddAsync(int taskId, int userId, string text)
    {
        var now = DateTime.UtcNow;
        var comment = new Comment
        {
            TaskId = taskId,
            UserId = userId,
            Text = text,
            CreatedAt = now,
            UpdatedAt = now,
            IsDeleted = false
        };
        return await _repo.AddAsync(comment);
    }

    public async Task UpdateAsync(int commentId, string text)
    {
        var comment = await _repo.GetByIdAsync(commentId, includeDeleted: true);
        if (comment == null) throw new InvalidOperationException("Comment not found.");
        var c = new Comment
        {
            CommentId = comment.CommentId,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            Text = text,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = comment.IsDeleted
        };
        await _repo.UpdateAsync(c);
    }

    public async Task DeleteAsync(int commentId)
    {
        var comment = await _repo.GetByIdAsync(commentId, includeDeleted: true);
        if (comment == null) throw new InvalidOperationException("Comment not found.");
        var c = new Comment
        {
            CommentId = comment.CommentId,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            Text = comment.Text,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = true
        };
        await _repo.UpdateAsync(c);
    }
}