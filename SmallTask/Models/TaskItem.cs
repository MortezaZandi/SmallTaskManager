namespace SmallTask.Models;

public enum TaskStatus
{
    Todo = 0,
    InProgress = 1,
    Done = 2,
    Finished = 3,
    Deleted = 4
}

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2
}

public class TaskItem
{
    public int TaskId { get; set; }
    public int TaskNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
    public int? AssignedUserId { get; set; }
    public int? GroupId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Project? Project { get; set; }
    public User? AssignedUser { get; set; }
    public Group? Group { get; set; }
    public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
