using SmallTask.Models;

namespace SmallTask.Services;

public interface ILabelService
{
    Task<Label?> GetByIdAsync(int labelId);
    Task<IReadOnlyList<Label>> GetAllAsync();
    Task<Label> CreateAsync(string name, string? description, string color);
    Task UpdateAsync(int labelId, string name, string? description, string color);
}
