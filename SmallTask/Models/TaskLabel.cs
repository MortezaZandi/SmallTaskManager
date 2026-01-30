namespace SmallTask.Models;

public class TaskLabel
{
    public int TaskId { get; set; }
    public int LabelId { get; set; }

    public TaskItem Task { get; set; } = null!;
    public Label Label { get; set; } = null!;
}
