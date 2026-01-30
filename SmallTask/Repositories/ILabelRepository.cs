using SmallTask.Models;

namespace SmallTask.Repositories;

public interface ILabelRepository
{
    Task<Label?> GetByIdAsync(int labelId);
    Task<IReadOnlyList<Label>> GetAllAsync();
    Task<Label> AddAsync(Label label);
    Task UpdateAsync(Label label);
}
