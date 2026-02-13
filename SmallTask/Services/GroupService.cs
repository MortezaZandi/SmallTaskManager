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

    public async Task<IReadOnlyList<Group>> GetRootGroupsAsync(int projectId) => await _groupRepo.GetRootGroupsAsync(projectId);

    public async Task<IReadOnlyList<Group>> GetChildrenAsync(int projectId, int parentGroupId) =>
        await _groupRepo.GetChildrenAsync(projectId, parentGroupId);

    public async Task<IReadOnlyList<Group>> GetAllFlatAsync(int projectId) => await _groupRepo.GetAllFlatAsync(projectId);

    public async Task<IReadOnlyList<GroupWithTaskCount>> GetAllWithTaskCountAsync(int projectId)
    {
        var groups = await _groupRepo.GetAllFlatAsync(projectId);
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

    public async Task<Group> CreateAsync(int projectId, string name, int? parentGroupId = null)
    {
        var group = new Group
        {
            ProjectId = projectId,
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
            ProjectId = group.ProjectId,
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
            ProjectId = group.ProjectId,
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
            ProjectId = group.ProjectId,
            ParentGroupId = group.ParentGroupId,
            Name = group.Name,
            IsDeleted = true,
            CreatedAt = group.CreatedAt
        };
        await _groupRepo.UpdateAsync(g);
    }
}