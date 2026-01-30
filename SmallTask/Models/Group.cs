namespace SmallTask.Models;

public class Group
{
    public int GroupId { get; set; }
    public int? ParentGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public Group? ParentGroup { get; set; }
    public ICollection<Group> Children { get; set; } = new List<Group>();
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
