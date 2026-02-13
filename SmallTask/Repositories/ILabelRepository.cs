using SmallTask.Models;

namespace SmallTask.Repositories;

public interface ILabelRepository
{
    Task<Label?> GetByIdAsync(int labelId);
    Task<IReadOnlyList<Label>> GetAllAsync(int projectId);
    Task<Label> AddAsync(Label label);
    Task UpdateAsync(Label label);
    Task DeleteAsync(int labelId);
}
