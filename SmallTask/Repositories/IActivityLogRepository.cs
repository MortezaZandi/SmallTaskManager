using SmallTask.Models;

namespace SmallTask.Repositories;

public interface IActivityLogRepository
{
    Task<ActivityLog> AddAsync(ActivityLog log);
    Task<(IReadOnlyList<ActivityLog> Items, int Total)> GetPagedAsync(int? projectId, int? userId, int? taskId, int? taskNumber, int page, int pageSize);
    Task<ActivityLog?> GetLastByTaskIdAsync(int taskId);
    Task<IReadOnlyDictionary<int, ActivityLog>> GetLastByTaskIdsAsync(IReadOnlyList<int> taskIds);
}
