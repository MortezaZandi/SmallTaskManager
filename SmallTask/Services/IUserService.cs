using SmallTask.Models;
using SmallTask.Repositories;

namespace SmallTask.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(int userId);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<IReadOnlyList<UserWithTaskCounts>> GetUsersWithTaskCountsAsync();
    Task<User> CreateAsync(string name, string? iconPath = null);
    Task UpdateAsync(int userId, string name, string? iconPath = null);
    Task DeleteAsync(int userId);
}
