using Microsoft.EntityFrameworkCore;
using Tel.Egram.Services.Persistence.Entities;

namespace Tel.Egram.Services.Persistence;

public class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public DbSet<KeyValueEntity> Values { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KeyValueEntity>(m =>
        {
            m.ToTable("key_value");
                
            m.Property(v => v.Key);
            m.Property(v => v.Value);
                
            m.HasKey(v => v.Key);
        });
    }
}