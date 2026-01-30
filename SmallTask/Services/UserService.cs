using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo) => _repo = repo;

    public async Task<User?> GetByIdAsync(int userId) => await _repo.GetByIdAsync(userId);

    public async Task<IReadOnlyList<User>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<User> CreateAsync(string name, string? iconPath = null)
    {
        var user = new User
        {
            Name = name,
            IconPath = iconPath,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };
        return await _repo.AddAsync(user);
    }

    public async Task UpdateAsync(int userId, string name, string? iconPath = null)
    {
        var user = await _repo.GetByIdAsync(userId, includeDeleted: true);
        if (user == null) throw new InvalidOperationException("User not found.");
        user = new User
        {
            UserId = user.UserId,
            Name = name,
            IconPath = iconPath,
            IsDeleted = user.IsDeleted,
            CreatedAt = user.CreatedAt
        };
        await _repo.UpdateAsync(user);
    }

    public async Task DeleteAsync(int userId)
    {
        var user = await _repo.GetByIdAsync(userId, includeDeleted: true);
        if (user == null) throw new InvalidOperationException("User not found.");
        var soft = new User
        {
            UserId = user.UserId,
            Name = user.Name,
            IconPath = user.IconPath,
            IsDeleted = true,
            CreatedAt = user.CreatedAt
        };
        await _repo.UpdateAsync(soft);
    }
}