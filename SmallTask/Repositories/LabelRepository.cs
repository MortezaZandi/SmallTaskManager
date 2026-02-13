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

    public async Task<IReadOnlyList<Label>> GetAllAsync(int projectId) =>
        await _db.Labels.AsNoTracking()
            .Where(x => x.ProjectId == projectId)
            .OrderBy(x => x.Name)
            .ToListAsync();

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

    public async Task DeleteAsync(int labelId)
    {
        var label = await _db.Labels.FindAsync(labelId);
        if (label != null)
        {
            _db.Labels.Remove(label);
            await _db.SaveChangesAsync();
        }
    }
}
