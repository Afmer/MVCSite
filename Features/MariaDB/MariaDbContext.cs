using Microsoft.EntityFrameworkCore;
using MVCSite.Models;
using MVCSite.Features.Extensions;
using MVCSite.Features.Enums;
using System.Text;

namespace MVCSite.Features.MariaDB;

public partial class MariaDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DbSet<UserIdentityDataModel> UserIdentity {get; set;} = null!;
    public MariaDbContext(DbContextOptions<MariaDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserIdentityDataModel>().HasData(
            new UserIdentityDataModel("admin", HashPassword.ComputePasswordHash("admin", 785433), 785433, Role.Admin)
        );
    }
}