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
