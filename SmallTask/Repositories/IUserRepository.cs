using SmallTask.Models;

namespace SmallTask.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId, bool includeDeleted = false);
    Task<IReadOnlyList<User>> GetAllAsync(bool includeDeleted = false);
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
}
