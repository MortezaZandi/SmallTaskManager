using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;
using TaskStatus = SmallTask.Models.TaskStatus;

namespace SmallTask.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _db;

    public TaskRepository(AppDbContext db) => _db = db;

    public async Task<TaskItem?> GetByIdAsync(int taskId, bool includeDeleted = false)
    {
        var q = _db.Tasks.AsNoTracking()
            .Include(x => x.AssignedUser)
            .Include(x => x.Group)
            .Include(x => x.TaskLabels).ThenInclude(x => x.Label)
            .Where(x => x.TaskId == taskId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.FirstOrDefaultAsync();
    }

    public async Task<int> GetNextTaskNumberAsync()
    {
        var max = await _db.Tasks.MaxAsync(x => (int?)x.TaskNumber) ?? 0;
        return max + 1;
    }

    public async Task<IReadOnlyList<TaskItem>> GetFilteredAsync(TaskFilter filter)
    {
        var q = _db.Tasks.AsNoTracking()
            .Include(x => x.AssignedUser)
            .Include(x => x.Group)
            .Include(x => x.TaskLabels).ThenInclude(x => x.Label)
            .Where(x => !x.IsDeleted && x.Status != TaskStatus.Deleted);

        if (!string.IsNullOrWhiteSpace(filter.Text))
        {
            var t = filter.Text.Trim().ToLower();
            q = q.Where(x =>
                x.Title.ToLower().Contains(t) ||
                (x.Description != null && x.Description.ToLower().Contains(t)) ||
                x.TaskNumber.ToString().Contains(t));
        }
        if (filter.GroupId.HasValue)
            q = q.Where(x => x.GroupId == filter.GroupId.Value);
        if (filter.LabelId.HasValue)
            q = q.Where(x => x.TaskLabels.Any(tl => tl.LabelId == filter.LabelId.Value));
        if (filter.Priority.HasValue)
            q = q.Where(x => x.Priority == filter.Priority.Value);
        if (filter.Status.HasValue)
            q = q.Where(x => x.Status == filter.Status.Value);
        if (filter.AssignedUserId.HasValue)
            q = q.Where(x => x.AssignedUserId == filter.AssignedUserId.Value);
        if (filter.TaskNumber.HasValue)
            q = q.Where(x => x.TaskNumber == filter.TaskNumber.Value);

        return await q.OrderBy(x => x.TaskNumber).ToListAsync();
    }

    public async Task<TaskItem> AddAsync(TaskItem task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _db.Tasks.Update(task);
        await _db.SaveChangesAsync();
    }

    public async Task SetTaskLabelsAsync(int taskId, IReadOnlyList<int> labelIds)
    {
        var existing = await _db.TaskLabels.Where(x => x.TaskId == taskId).ToListAsync();
        _db.TaskLabels.RemoveRange(existing);
        foreach (var labelId in labelIds.Distinct())
            _db.TaskLabels.Add(new TaskLabel { TaskId = taskId, LabelId = labelId });
        await _db.SaveChangesAsync();
    }
}
