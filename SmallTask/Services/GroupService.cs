using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepo;
    private readonly ITaskRepository _taskRepo;

    public GroupService(IGroupRepository groupRepo, ITaskRepository taskRepo)
    {
        _groupRepo = groupRepo;
        _taskRepo = taskRepo;
    }

    public async Task<Group?> GetByIdAsync(int groupId) => await _groupRepo.GetByIdAsync(groupId);

    public async Task<IReadOnlyList<Group>> GetRootGroupsAsync() => await _groupRepo.GetRootGroupsAsync();

    public async Task<IReadOnlyList<Group>> GetChildrenAsync(int parentGroupId) =>
        await _groupRepo.GetChildrenAsync(parentGroupId);

    public async Task<IReadOnlyList<Group>> GetAllFlatAsync() => await _groupRepo.GetAllFlatAsync();

    public async Task<IReadOnlyList<GroupWithTaskCount>> GetAllWithTaskCountAsync(int? projectId = null)
    {
        var groups = await _groupRepo.GetAllFlatAsync();
        var result = new List<GroupWithTaskCount>();
        foreach (var g in groups)
        {
            var countInProject = await _groupRepo.GetTaskCountAsync(g.GroupId, projectId);
            var countTotal = await _groupRepo.GetTaskCountAsync(g.GroupId, null);
            result.Add(new GroupWithTaskCount
            {
                GroupId = g.GroupId,
                Name = g.Name,
                ParentGroupId = g.ParentGroupId,
                TaskCount = countInProject,
                TaskCountTotal = countTotal
            });
        }
        return result;
    }

    public async Task<Group> CreateAsync(string name, int? parentGroupId = null)
    {
        var group = new Group
        {
            ParentGroupId = parentGroupId,
            Name = name,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };
        return await _groupRepo.AddAsync(group);
    }

    public async Task RenameAsync(int groupId, string name)
    {
        var group = await _groupRepo.GetByIdAsync(groupId, includeDeleted: true);
        if (group == null) throw new InvalidOperationException("Group not found.");
        var g = new Group
        {
            GroupId = group.GroupId,
            ParentGroupId = group.ParentGroupId,
            Name = name,
            IsDeleted = group.IsDeleted,
            CreatedAt = group.CreatedAt
        };
        await _groupRepo.UpdateAsync(g);
    }

    public async Task MoveAsync(int groupId, int? newParentGroupId)
    {
        var group = await _groupRepo.GetByIdAsync(groupId, includeDeleted: true);
        if (group == null) throw new InvalidOperationException("Group not found.");
        if (newParentGroupId == groupId) throw new InvalidOperationException("Group cannot be parent of itself.");
        if (newParentGroupId.HasValue)
        {
            var ancestor = newParentGroupId;
            while (ancestor.HasValue)
            {
                if (ancestor.Value == groupId) throw new InvalidOperationException("Cannot move group into its own descendant.");
                var p = await _groupRepo.GetByIdAsync(ancestor.Value, includeDeleted: true);
                ancestor = p?.ParentGroupId;
            }
        }
        var g = new Group
        {
            GroupId = group.GroupId,
            ParentGroupId = newParentGroupId,
            Name = group.Name,
            IsDeleted = group.IsDeleted,
            CreatedAt = group.CreatedAt
        };
        await _groupRepo.UpdateAsync(g);
    }

    public async Task DeleteAsync(int groupId)
    {
        var count = await _groupRepo.GetTaskCountAsync(groupId);
        if (count > 0) throw new InvalidOperationException("Cannot delete group that contains tasks.");
        var group = await _groupRepo.GetByIdAsync(groupId, includeDeleted: true);
        if (group == null) throw new InvalidOperationException("Group not found.");
        var g = new Group
        {
            GroupId = group.GroupId,
            ParentGroupId = group.ParentGroupId,
            Name = group.Name,
            IsDeleted = true,
            CreatedAt = group.CreatedAt
        };
        await _groupRepo.UpdateAsync(g);
    }
}