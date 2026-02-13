using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public class LabelService : ILabelService
{
    private readonly ILabelRepository _repo;

    public LabelService(ILabelRepository repo) => _repo = repo;

    public async Task<Label?> GetByIdAsync(int labelId) => await _repo.GetByIdAsync(labelId);

    public async Task<IReadOnlyList<Label>> GetAllAsync(int projectId) => await _repo.GetAllAsync(projectId);

    public async Task<Label> CreateAsync(int projectId, string name, string? description, string color)
    {
        var label = new Label { ProjectId = projectId, Name = name, Description = description, Color = color };
        return await _repo.AddAsync(label);
    }

    public async Task UpdateAsync(int labelId, string name, string? description, string color)
    {
        var label = await _repo.GetByIdAsync(labelId);
        if (label == null) throw new InvalidOperationException("Label not found.");
        var l = new Label
        {
            LabelId = label.LabelId,
            ProjectId = label.ProjectId,
            Name = name,
            Description = description,
            Color = color
        };
        await _repo.UpdateAsync(l);
    }

    public async Task DeleteAsync(int labelId)
    {
        var label = await _repo.GetByIdAsync(labelId);
        if (label == null) throw new InvalidOperationException("Label not found.");
        await _repo.DeleteAsync(labelId);
    }
}