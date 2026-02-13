using SmallTask.Models;
using SmallTask.Repositories;
using TaskStatus = SmallTask.Models.TaskStatus;

namespace SmallTask.Services;

public interface ITaskService
{
    Task<TaskItem?> GetByIdAsync(int taskId);
    Task<IReadOnlyList<TaskItem>> GetFilteredAsync(TaskFilter filter);
    Task<TaskItem> CreateAsync(int projectId, string title, string? description, TaskStatus status, TaskPriority priority, int? assignedUserId, int? groupId, IReadOnlyList<int>? labelIds = null);
    Task UpdateAsync(int taskId, int projectId, string title, string? description, TaskStatus status, TaskPriority priority, int? assignedUserId, int? groupId, IReadOnlyList<int>? labelIds = null);
    Task DeleteAsync(int taskId);
}
