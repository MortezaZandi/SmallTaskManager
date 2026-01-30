namespace SmallTask.Settings;

public class AppSettings
{
    public const string SectionName = "AppSettings";

    public string ConnectionString { get; set; } = "Server=(localdb)\\mssqllocaldb;Database=SmallTaskDb;Trusted_Connection=True;MultipleActiveResultSets=true";
    public string AttachmentStoragePath { get; set; } = "Attachments";
}
