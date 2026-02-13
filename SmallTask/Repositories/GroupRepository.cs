using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;
using TaskStatus = SmallTask.Models.TaskStatus;

namespace SmallTask.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly AppDbContext _db;

    public GroupRepository(AppDbContext db) => _db = db;

    public async Task<Group?> GetByIdAsync(int groupId, bool includeDeleted = false)
    {
        var q = _db.Groups.AsNoTracking()
            .Include(x => x.ParentGroup)
            .Where(x => x.GroupId == groupId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<Group>> GetRootGroupsAsync(int projectId, bool includeDeleted = false)
    {
        var q = _db.Groups.AsNoTracking()
            .Where(x => x.ProjectId == projectId && x.ParentGroupId == null);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<IReadOnlyList<Group>> GetChildrenAsync(int projectId, int parentGroupId, bool includeDeleted = false)
    {
        var q = _db.Groups.AsNoTracking()
            .Where(x => x.ProjectId == projectId && x.ParentGroupId == parentGroupId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<IReadOnlyList<Group>> GetAllFlatAsync(int projectId, bool includeDeleted = false)
    {
        var q = _db.Groups.AsNoTracking().Where(x => x.ProjectId == projectId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.OrderBy(x => x.Name).ToListAsync();
    }

    public async Task<int> GetTaskCountAsync(int groupId, int? projectId = null, bool includeDeletedTasks = false)
    {
        var q = _db.Tasks.Where(x => x.GroupId == groupId);
        if (projectId.HasValue) q = q.Where(x => x.ProjectId == projectId.Value);
        if (!includeDeletedTasks) q = q.Where(x => !x.IsDeleted && x.Status != TaskStatus.Deleted);
        return await q.CountAsync();
    }

    public async Task<Group> AddAsync(Group group)
    {
        _db.Groups.Add(group);
        await _db.SaveChangesAsync();
        return group;
    }

    public async Task UpdateAsync(Group group)
    {
        _db.Groups.Update(group);
        await _db.SaveChangesAsync();
    }
}
