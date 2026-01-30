using SmallTask.Models;

namespace SmallTask.Services;

public interface IGroupService
{
    Task<Group?> GetByIdAsync(int groupId);
    Task<IReadOnlyList<Group>> GetRootGroupsAsync();
    Task<IReadOnlyList<Group>> GetChildrenAsync(int parentGroupId);
    Task<IReadOnlyList<Group>> GetAllFlatAsync();
    Task<Group> CreateAsync(string name, int? parentGroupId = null);
    Task RenameAsync(int groupId, string name);
    Task MoveAsync(int groupId, int? newParentGroupId);
    Task DeleteAsync(int groupId);
}
