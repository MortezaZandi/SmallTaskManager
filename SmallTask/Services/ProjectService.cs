using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepo;

    public ProjectService(IProjectRepository projectRepo) => _projectRepo = projectRepo;

    public async Task<Project?> GetByIdAsync(int projectId) =>
        await _projectRepo.GetByIdAsync(projectId);

    public async Task<IReadOnlyList<ProjectWithTaskCount>> GetAllWithTaskCountAsync()
    {
        var projects = await _projectRepo.GetAllAsync();
        var result = new List<ProjectWithTaskCount>();
        foreach (var p in projects)
        {
            var count = await _projectRepo.GetTaskCountAsync(p.ProjectId);
            result.Add(new ProjectWithTaskCount
            {
                ProjectId = p.ProjectId,
                Name = p.Name,
                Description = p.Description,
                IsDeleted = p.IsDeleted,
                TaskCount = count
            });
        }
        return result;
    }

    public async Task<Project> CreateAsync(string name, string? description = null)
    {
        var project = new Project
        {
            Name = name,
            Description = description,
            IsDeleted = false
        };
        return await _projectRepo.AddAsync(project);
    }

    public async Task DeleteAsync(int projectId)
    {
        var project = await _projectRepo.GetByIdAsync(projectId, includeDeleted: true);
        if (project == null) throw new InvalidOperationException("Project not found.");
        var count = await _projectRepo.GetTaskCountAsync(projectId);
        if (count > 0) throw new InvalidOperationException("Cannot delete project that contains tasks. Move or delete the tasks first.");
        var p = new Project
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            Description = project.Description,
            IsDeleted = true
        };
        await _projectRepo.UpdateAsync(p);
    }
}
