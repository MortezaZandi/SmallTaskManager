using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;

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

    public async Task<IReadOnlyList<Group>> GetRootGroupsAsync(bool includeDeleted = false)
    {
        var q = _db.Groups.AsNoTracking()
            .Where(x => x.ParentGroupId == null)
            .OrderBy(x => x.Name);
        if (!includeDeleted) q = (IOrderedQueryable<Group>)q.Where(x => !x.IsDeleted);
        return await q.ToListAsync();
    }

    public async Task<IReadOnlyList<Group>> GetChildrenAsync(int parentGroupId, bool includeDeleted = false)
    {
        var q = _db.Groups.AsNoTracking()
            .Where(x => x.ParentGroupId == parentGroupId)
            .OrderBy(x => x.Name);
        if (!includeDeleted) q = (IOrderedQueryable<Group>)q.Where(x => !x.IsDeleted);
        return await q.ToListAsync();
    }

    public async Task<IReadOnlyList<Group>> GetAllFlatAsync(bool includeDeleted = false)
    {
        var q = _db.Groups.AsNoTracking().OrderBy(x => x.Name);
        if (!includeDeleted) q = (IOrderedQueryable<Group>)q.Where(x => !x.IsDeleted);
        return await q.ToListAsync();
    }

    public async Task<int> GetTaskCountAsync(int groupId, bool includeDeletedTasks = false)
    {
        var q = _db.Tasks.Where(x => x.GroupId == groupId);
        if (!includeDeletedTasks) q = q.Where(x => !x.IsDeleted);
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
