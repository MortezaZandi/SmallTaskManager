namespace SmallTask.Models;

public class Comment
{
    public int CommentId { get; set; }
    public int TaskId { get; set; }
    public int UserId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public TaskItem Task { get; set; } = null!;
    public User User { get; set; } = null!;
}
