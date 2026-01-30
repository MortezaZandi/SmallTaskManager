using SmallTask.Models;

namespace SmallTask.Repositories;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(int groupId, bool includeDeleted = false);
    Task<IReadOnlyList<Group>> GetRootGroupsAsync(bool includeDeleted = false);
    Task<IReadOnlyList<Group>> GetChildrenAsync(int parentGroupId, bool includeDeleted = false);
    Task<IReadOnlyList<Group>> GetAllFlatAsync(bool includeDeleted = false);
    Task<int> GetTaskCountAsync(int groupId, bool includeDeletedTasks = false);
    Task<Group> AddAsync(Group group);
    Task UpdateAsync(Group group);
}
