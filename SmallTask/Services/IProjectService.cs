using SmallTask.Models;

namespace SmallTask.Services;

public interface IProjectService
{
    Task<Project?> GetByIdAsync(int projectId);
    Task<IReadOnlyList<ProjectWithTaskCount>> GetAllWithTaskCountAsync();
    Task<Project> CreateAsync(string name, string? description = null, string? iconPath = null);
    Task UpdateAsync(int projectId, string name, string? description = null, string? iconPath = null);
    Task DeleteAsync(int projectId);
}

public class ProjectWithTaskCount
{
    public int ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? IconPath { get; set; }
    public bool IsDeleted { get; set; }
    public int TaskCount { get; set; }
}
