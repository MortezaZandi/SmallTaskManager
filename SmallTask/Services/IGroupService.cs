using SmallTask.Models;

namespace SmallTask.Services;

public class GroupWithTaskCount
{
    public int GroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? ParentGroupId { get; set; }
    public int TaskCount { get; set; }
    public int TaskCountTotal { get; set; }
}

public interface IGroupService
{
    Task<Group?> GetByIdAsync(int groupId);
    Task<IReadOnlyList<Group>> GetRootGroupsAsync(int projectId);
    Task<IReadOnlyList<Group>> GetChildrenAsync(int projectId, int parentGroupId);
    Task<IReadOnlyList<Group>> GetAllFlatAsync(int projectId);
    Task<IReadOnlyList<GroupWithTaskCount>> GetAllWithTaskCountAsync(int projectId);
    Task<Group> CreateAsync(int projectId, string name, int? parentGroupId = null);
    Task RenameAsync(int groupId, string name);
    Task MoveAsync(int groupId, int? newParentGroupId);
    Task DeleteAsync(int groupId);
}
