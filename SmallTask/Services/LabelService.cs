using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public class LabelService : ILabelService
{
    private readonly ILabelRepository _repo;

    public LabelService(ILabelRepository repo) => _repo = repo;

    public async Task<Label?> GetByIdAsync(int labelId) => await _repo.GetByIdAsync(labelId);

    public async Task<IReadOnlyList<Label>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Label> CreateAsync(string name, string? description, string color)
    {
        var label = new Label { Name = name, Description = description, Color = color };
        return await _repo.AddAsync(label);
    }

    public async Task UpdateAsync(int labelId, string name, string? description, string color)
    {
        var label = await _repo.GetByIdAsync(labelId);
        if (label == null) throw new InvalidOperationException("Label not found.");
        var l = new Label
        {
            LabelId = label.LabelId,
            Name = name,
            Description = description,
            Color = color
        };
        await _repo.UpdateAsync(l);
    }
}