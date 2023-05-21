using Microsoft.EntityFrameworkCore;

public partial class MariaDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public MariaDbContext(DbContextOptions<MariaDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}