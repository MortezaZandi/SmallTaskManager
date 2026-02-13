using SmallTask.Models;

namespace SmallTask.Services;

public interface IActivityLogService
{
    Task LogAsync(int projectId, int taskId, int? userId, string actionDetails);
    Task<(IReadOnlyList<ActivityLogDto> Items, int Total)> GetPagedAsync(int? projectId, int? userId, int? taskId, int? taskNumber, int page, int pageSize);
    Task<ActivityLogDto?> GetLastByTaskIdAsync(int taskId);
    Task<IReadOnlyDictionary<int, ActivityLogDto>> GetLastByTaskIdsAsync(IReadOnlyList<int> taskIds);
}

public class ActivityLogDto
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public int ProjectId { get; set; }
    public string? ProjectName { get; set; }
    public int? UserId { get; set; }
    public string? UserName { get; set; }
    public int TaskId { get; set; }
    public int? TaskNumber { get; set; }
    public string? TaskTitle { get; set; }
    public string ActionDetails { get; set; } = string.Empty;
}
