namespace Tel.Egram.Services.Persistence;

public interface IDatabaseContextFactory
{
    DatabaseContext CreateDbContext();
}