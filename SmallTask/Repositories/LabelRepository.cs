using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;

namespace SmallTask.Repositories;

public class LabelRepository : ILabelRepository
{
    private readonly AppDbContext _db;

    public LabelRepository(AppDbContext db) => _db = db;

    public async Task<Label?> GetByIdAsync(int labelId) =>
        await _db.Labels.AsNoTracking().FirstOrDefaultAsync(x => x.LabelId == labelId);

    public async Task<IReadOnlyList<Label>> GetAllAsync() =>
        await _db.Labels.AsNoTracking().OrderBy(x => x.Name).ToListAsync();

    public async Task<Label> AddAsync(Label label)
    {
        _db.Labels.Add(label);
        await _db.SaveChangesAsync();
        return label;
    }

    public async Task UpdateAsync(Label label)
    {
        _db.Labels.Update(label);
        await _db.SaveChangesAsync();
    }
}
