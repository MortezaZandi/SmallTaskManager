namespace SmallTask.Models;

public class Project
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconPath { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Label> Labels { get; set; } = new List<Label>();
}
