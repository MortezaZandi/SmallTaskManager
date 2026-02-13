using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;
using TaskStatus = SmallTask.Models.TaskStatus;

namespace SmallTask.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _db;

    public ProjectRepository(AppDbContext db) => _db = db;

    public async Task<Project?> GetByIdAsync(int projectId, bool includeDeleted = false)
    {
        var q = _db.Projects.AsNoTracking().Where(x => x.ProjectId == projectId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync(bool includeDeleted = false)
    {
        var q = _db.Projects.AsNoTracking();
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<int> GetTaskCountAsync(int projectId, bool includeDeletedTasks = false)
    {
        var q = _db.Tasks.Where(x => x.ProjectId == projectId);
        if (!includeDeletedTasks) q = q.Where(x => !x.IsDeleted && x.Status != TaskStatus.Deleted);
        return await q.CountAsync();
    }

    public async Task<Project> AddAsync(Project project)
    {
        _db.Projects.Add(project);
        await _db.SaveChangesAsync();
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        _db.Projects.Update(project);
        await _db.SaveChangesAsync();
    }
}
