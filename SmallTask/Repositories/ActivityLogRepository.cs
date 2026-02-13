using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;

namespace SmallTask.Repositories;

public class ActivityLogRepository : IActivityLogRepository
{
    private readonly AppDbContext _db;

    public ActivityLogRepository(AppDbContext db) => _db = db;

    public async Task<ActivityLog> AddAsync(ActivityLog log)
    {
        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();
        return log;
    }

    public async Task<(IReadOnlyList<ActivityLog> Items, int Total)> GetPagedAsync(int? projectId, int? userId, int? taskId, int? taskNumber, int page, int pageSize)
    {
        IQueryable<ActivityLog> q = _db.ActivityLogs.AsNoTracking()
            .Include(x => x.Project)
            .Include(x => x.User)
            .Include(x => x.Task);

        if (projectId.HasValue) q = q.Where(x => x.ProjectId == projectId.Value);
        if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
        if (taskId.HasValue) q = q.Where(x => x.TaskId == taskId.Value);
        if (taskNumber.HasValue) q = q.Where(x => x.Task != null && x.Task.TaskNumber == taskNumber.Value);

        var total = await q.CountAsync();
        var items = await q.OrderByDescending(x => x.Date).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        return (items, total);
    }

    public async Task<ActivityLog?> GetLastByTaskIdAsync(int taskId) =>
        await _db.ActivityLogs.AsNoTracking()
            .Include(x => x.User)
            .Where(x => x.TaskId == taskId)
            .OrderByDescending(x => x.Date)
            .FirstOrDefaultAsync();

    public async Task<IReadOnlyDictionary<int, ActivityLog>> GetLastByTaskIdsAsync(IReadOnlyList<int> taskIds)
    {
        if (taskIds == null || taskIds.Count == 0) return new Dictionary<int, ActivityLog>();
        var ids = taskIds.Distinct().ToList();
        var logs = await _db.ActivityLogs.AsNoTracking()
            .Include(x => x.User)
            .Where(x => ids.Contains(x.TaskId))
            .OrderByDescending(x => x.Date)
            .ToListAsync();
        return logs.GroupBy(x => x.TaskId).ToDictionary(g => g.Key, g => g.First());
    }
}
