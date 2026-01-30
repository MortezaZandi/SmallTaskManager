namespace SmallTask.Models;

public class User
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? IconPath { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
