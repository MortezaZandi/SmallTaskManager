namespace SmallTask.Models;

public class Label
{
    public int LabelId { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#000000";

    public Project Project { get; set; } = null!;
    public ICollection<TaskLabel> TaskLabels { get; set; } = new List<TaskLabel>();
}
