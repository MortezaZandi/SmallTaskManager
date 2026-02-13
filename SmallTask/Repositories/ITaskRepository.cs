using SmallTask.Models;
using TaskStatus = SmallTask.Models.TaskStatus;

namespace SmallTask.Repositories;

public class TaskFilter
{
    public int? ProjectId { get; set; }
    public string? Text { get; set; }
    public int? GroupId { get; set; }
    public int? LabelId { get; set; }
    public TaskPriority? Priority { get; set; }
    public TaskStatus? Status { get; set; }
    public int? AssignedUserId { get; set; }
    public int? TaskNumber { get; set; }
}

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(int taskId, bool includeDeleted = false);
    Task<int> GetNextTaskNumberAsync();
    Task<IReadOnlyList<TaskItem>> GetFilteredAsync(TaskFilter filter);
    Task<TaskItem> AddAsync(TaskItem task);
    Task UpdateAsync(TaskItem task);
    Task SetTaskLabelsAsync(int taskId, IReadOnlyList<int> labelIds);
}
