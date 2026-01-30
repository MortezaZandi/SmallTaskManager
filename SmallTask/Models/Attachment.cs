namespace SmallTask.Models;

public class Attachment
{
    public int AttachmentId { get; set; }
    public int TaskId { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsDeleted { get; set; }

    public TaskItem Task { get; set; } = null!;
}
