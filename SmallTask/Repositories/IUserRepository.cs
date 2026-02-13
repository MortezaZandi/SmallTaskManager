using SmallTask.Models;

namespace SmallTask.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId, bool includeDeleted = false);
    Task<IReadOnlyList<User>> GetAllAsync(bool includeDeleted = false);
    Task<IReadOnlyList<UserWithTaskCounts>> GetUsersWithTaskCountsAsync();
    Task<User> AddAsync(User user);
    Task UpdateAsync(User user);
}

public record UserWithTaskCounts(int UserId, string Name, string? IconPath, int TaskCountTodo, int TaskCountInProgress, int TaskCountDone);
