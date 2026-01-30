using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SmallTask.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>();

        options.UseSqlServer(
            "Server=AsusZ390TUF;Database=SmallTask;Trusted_Connection=True;TrustServerCertificate=True"
            , sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
        );

        return new AppDbContext(options.Options);
    }


}
