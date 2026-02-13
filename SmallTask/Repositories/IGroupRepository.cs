using SmallTask.Models;

namespace SmallTask.Repositories;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(int groupId, bool includeDeleted = false);
    Task<IReadOnlyList<Group>> GetRootGroupsAsync(int projectId, bool includeDeleted = false);
    Task<IReadOnlyList<Group>> GetChildrenAsync(int projectId, int parentGroupId, bool includeDeleted = false);
    Task<IReadOnlyList<Group>> GetAllFlatAsync(int projectId, bool includeDeleted = false);
    Task<int> GetTaskCountAsync(int groupId, int? projectId = null, bool includeDeletedTasks = false);
    Task<Group> AddAsync(Group group);
    Task UpdateAsync(Group group);
}
