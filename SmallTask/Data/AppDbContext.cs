using Microsoft.EntityFrameworkCore;
using SmallTask.Models;

namespace SmallTask.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<User> Users => Set<User>();
        public DbSet<TaskItem> Tasks => Set<TaskItem>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Label> Labels => Set<Label>();
        public DbSet<TaskLabel> TaskLabels => Set<TaskLabel>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Attachment> Attachments => Set<Attachment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>(e =>
            {
                e.HasKey(x => x.ProjectId);
                e.Property(x => x.Name).HasMaxLength(200);
                e.Property(x => x.Description).HasMaxLength(2000);
            });

            modelBuilder.Entity<User>(e =>
            {
                e.HasKey(x => x.UserId);
                e.Property(x => x.Name).HasMaxLength(200);
                e.Property(x => x.IconPath).HasMaxLength(500);
            });

            modelBuilder.Entity<Group>(e =>
            {
                e.HasKey(x => x.GroupId);
                e.Property(x => x.Name).HasMaxLength(200);
                e.HasOne(x => x.ParentGroup)
                    .WithMany(x => x.Children)
                    .HasForeignKey(x => x.ParentGroupId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Label>(e =>
            {
                e.HasKey(x => x.LabelId);
                e.Property(x => x.Name).HasMaxLength(100);
                e.Property(x => x.Description).HasMaxLength(500);
                e.Property(x => x.Color).HasMaxLength(20);
            });

            modelBuilder.Entity<TaskItem>(e =>
            {
                e.HasKey(x => x.TaskId);
                e.Property(x => x.Title).HasMaxLength(500);
                e.Property(x => x.Description).HasMaxLength(4000);
                e.HasIndex(x => x.TaskNumber).IsUnique();
                e.HasOne(x => x.Project)
                    .WithMany(x => x.Tasks)
                    .HasForeignKey(x => x.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict);
                e.HasOne(x => x.AssignedUser)
                    .WithMany()
                    .HasForeignKey(x => x.AssignedUserId)
                    .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(x => x.Group)
                    .WithMany(x => x.Tasks)
                    .HasForeignKey(x => x.GroupId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TaskLabel>(e =>
            {
                e.HasKey(x => new { x.TaskId, x.LabelId });
                e.HasOne(x => x.Task)
                    .WithMany(x => x.TaskLabels)
                    .HasForeignKey(x => x.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Label)
                    .WithMany(x => x.TaskLabels)
                    .HasForeignKey(x => x.LabelId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Comment>(e =>
            {
                e.HasKey(x => x.CommentId);
                e.Property(x => x.Text).HasMaxLength(4000);
                e.HasOne(x => x.Task)
                    .WithMany(x => x.Comments)
                    .HasForeignKey(x => x.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Attachment>(e =>
            {
                e.HasKey(x => x.AttachmentId);
                e.Property(x => x.OriginalFileName).HasMaxLength(500);
                e.Property(x => x.StoredFileName).HasMaxLength(600);
                e.Property(x => x.FilePath).HasMaxLength(1000);
                e.HasOne(x => x.Task)
                    .WithMany(x => x.Attachments)
                    .HasForeignKey(x => x.TaskId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}