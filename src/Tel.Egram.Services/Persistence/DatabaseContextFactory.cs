using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tel.Egram.Services.Persistence;

public class DatabaseContextFactory : IDatabaseContextFactory, IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var storage = new Storage();
        var options = new DbContextOptionsBuilder<DatabaseContext>();
        options.UseSqlite($"Data Source={storage.DatabaseFile};");
        return new DatabaseContext(options.Options);
    }

    public DatabaseContext CreateDbContext() => CreateDbContext([]);
}