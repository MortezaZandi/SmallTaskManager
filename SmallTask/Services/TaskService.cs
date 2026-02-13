using SmallTask.Models;
using SmallTask.Repositories;
using TaskStatus = SmallTask.Models.TaskStatus;

namespace SmallTask.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepo;

    public TaskService(ITaskRepository taskRepo) => _taskRepo = taskRepo;

    public async Task<TaskItem?> GetByIdAsync(int taskId) => await _taskRepo.GetByIdAsync(taskId);

    public async Task<IReadOnlyList<TaskItem>> GetFilteredAsync(TaskFilter filter) =>
        await _taskRepo.GetFilteredAsync(filter);

    public async Task<TaskItem> CreateAsync(int projectId, string title, string? description, TaskStatus status, TaskPriority priority, int? assignedUserId, int? groupId, IReadOnlyList<int>? labelIds = null)
    {
        var taskNumber = await _taskRepo.GetNextTaskNumberAsync();
        var now = DateTime.UtcNow;
        var task = new TaskItem
        {
            ProjectId = projectId,
            TaskNumber = taskNumber,
            Title = title,
            Description = description,
            Status = status,
            Priority = priority,
            AssignedUserId = assignedUserId,
            GroupId = groupId,
            IsDeleted = false,
            CreatedAt = now,
            UpdatedAt = now
        };
        task = await _taskRepo.AddAsync(task);
        if (labelIds != null && labelIds.Count > 0)
            await _taskRepo.SetTaskLabelsAsync(task.TaskId, labelIds);
        return (await _taskRepo.GetByIdAsync(task.TaskId))!;
    }

    public async Task UpdateAsync(int taskId, int projectId, string title, string? description, TaskStatus status, TaskPriority priority, int? assignedUserId, int? groupId, IReadOnlyList<int>? labelIds = null)
    {
        var task = await _taskRepo.GetByIdAsync(taskId, includeDeleted: true);
        if (task == null) throw new InvalidOperationException("Task not found.");
        var t = new TaskItem
        {
            TaskId = task.TaskId,
            ProjectId = projectId,
            TaskNumber = task.TaskNumber,
            Title = title,
            Description = description,
            Status = status,
            Priority = priority,
            AssignedUserId = assignedUserId,
            GroupId = groupId,
            IsDeleted = task.IsDeleted,
            CreatedAt = task.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
        await _taskRepo.UpdateAsync(t);
        if (labelIds != null)
            await _taskRepo.SetTaskLabelsAsync(taskId, labelIds);
    }

    public async Task DeleteAsync(int taskId)
    {
        var task = await _taskRepo.GetByIdAsync(taskId, includeDeleted: true);
        if (task == null) throw new InvalidOperationException("Task not found.");
        var t = new TaskItem
        {
            TaskId = task.TaskId,
            ProjectId = task.ProjectId,
            TaskNumber = task.TaskNumber,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            Priority = task.Priority,
            AssignedUserId = task.AssignedUserId,
            GroupId = task.GroupId,
            IsDeleted = true,
            CreatedAt = task.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
        await _taskRepo.UpdateAsync(t);
    }
}