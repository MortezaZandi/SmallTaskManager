using SmallTask.Models;

namespace SmallTask.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(int userId);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User> CreateAsync(string name, string? iconPath = null);
    Task UpdateAsync(int userId, string name, string? iconPath = null);
    Task DeleteAsync(int userId);
}
