namespace SmallTask.Models;

public class ActivityLog
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ProjectId { get; set; }
    public int? UserId { get; set; }
    public int TaskId { get; set; }
    public string ActionDetails { get; set; } = string.Empty;

    public Project? Project { get; set; }
    public User? User { get; set; }
    public TaskItem? Task { get; set; }
}
