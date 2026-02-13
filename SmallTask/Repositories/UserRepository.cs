using Microsoft.EntityFrameworkCore;
using SmallTask.Data;
using SmallTask.Models;

namespace SmallTask.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(int userId, bool includeDeleted = false)
    {
        var q = _db.Users.AsNoTracking().Where(x => x.UserId == userId);
        if (!includeDeleted) q = q.Where(x => !x.IsDeleted);
        return await q.FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(bool includeDeleted = false)
    {
        var q = _db.Users.AsNoTracking().OrderBy(x => x.Name);
        if (!includeDeleted) q = (IOrderedQueryable<User>)q.Where(x => !x.IsDeleted);
        return await q.ToListAsync();
    }

    public async Task<IReadOnlyList<UserWithTaskCounts>> GetUsersWithTaskCountsAsync()
    {
        var users = await _db.Users.AsNoTracking()
            .Where(u => !u.IsDeleted)
            .OrderBy(u => u.Name)
            .Select(u => new { u.UserId, u.Name, u.IconPath })
            .ToListAsync();

        var counts = await _db.Tasks.AsNoTracking()
            .Where(t => t.AssignedUserId != null && !t.IsDeleted && (int)t.Status <= 2)
            .GroupBy(t => new { t.AssignedUserId, t.Status })
            .Select(g => new { g.Key.AssignedUserId, g.Key.Status, Count = g.Count() })
            .ToListAsync();

        var dict = counts
            .GroupBy(c => c.AssignedUserId!.Value)
            .ToDictionary(g => g.Key, g => g.ToDictionary(x => x.Status, x => x.Count));

        return users.Select(u =>
        {
            dict.TryGetValue(u.UserId, out var statusCounts);
            return new UserWithTaskCounts(
                u.UserId,
                u.Name,
                u.IconPath,
                statusCounts?.GetValueOrDefault(Models.TaskStatus.Todo) ?? 0,
                statusCounts?.GetValueOrDefault(Models.TaskStatus.InProgress) ?? 0,
                statusCounts?.GetValueOrDefault(Models.TaskStatus.Done) ?? 0
            );
        }).ToList();
    }

    public async Task<User> AddAsync(User user)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }
}
