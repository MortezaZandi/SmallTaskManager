using SmallTask.Models;

namespace SmallTask.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int projectId, bool includeDeleted = false);
    Task<IReadOnlyList<Project>> GetAllAsync(bool includeDeleted = false);
    Task<int> GetTaskCountAsync(int projectId, bool includeDeletedTasks = false);
    Task<Project> AddAsync(Project project);
    Task UpdateAsync(Project project);
}
