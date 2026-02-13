using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public class ActivityLogService : IActivityLogService
{
    private readonly IActivityLogRepository _repo;

    public ActivityLogService(IActivityLogRepository repo) => _repo = repo;

    public async Task LogAsync(int projectId, int taskId, int? userId, string actionDetails)
    {
        var log = new ActivityLog
        {
            Date = DateTime.UtcNow,
            ProjectId = projectId,
            UserId = userId,
            TaskId = taskId,
            ActionDetails = actionDetails
        };
        await _repo.AddAsync(log);
    }

    public async Task<(IReadOnlyList<ActivityLogDto> Items, int Total)> GetPagedAsync(int? projectId, int? userId, int? taskId, int? taskNumber, int page, int pageSize)
    {
        var (items, total) = await _repo.GetPagedAsync(projectId, userId, taskId, taskNumber, page, pageSize);
        var dtos = items.Select(x => new ActivityLogDto
        {
            Id = x.Id,
            Date = DateTime.SpecifyKind(x.Date, DateTimeKind.Utc),
            ProjectId = x.ProjectId,
            ProjectName = x.Project?.Name,
            UserId = x.UserId,
            UserName = x.User?.Name,
            TaskId = x.TaskId,
            TaskNumber = x.Task?.TaskNumber,
            TaskTitle = x.Task?.Title,
            ActionDetails = x.ActionDetails
        }).ToList();
        return (dtos, total);
    }

    public async Task<ActivityLogDto?> GetLastByTaskIdAsync(int taskId)
    {
        var log = await _repo.GetLastByTaskIdAsync(taskId);
        return log == null ? null : ToDto(log);
    }

    public async Task<IReadOnlyDictionary<int, ActivityLogDto>> GetLastByTaskIdsAsync(IReadOnlyList<int> taskIds)
    {
        var logs = await _repo.GetLastByTaskIdsAsync(taskIds);
        return logs.ToDictionary(x => x.Key, x => ToDto(x.Value));
    }

    private static ActivityLogDto ToDto(ActivityLog log) => new()
    {
        Id = log.Id,
        Date = DateTime.SpecifyKind(log.Date, DateTimeKind.Utc),
        ProjectId = log.ProjectId,
        ProjectName = log.Project?.Name,
        UserId = log.UserId,
        UserName = log.User?.Name,
        TaskId = log.TaskId,
        TaskNumber = log.Task?.TaskNumber,
        TaskTitle = log.Task?.Title,
        ActionDetails = log.ActionDetails
    };
}
