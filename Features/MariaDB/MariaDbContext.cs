using Microsoft.EntityFrameworkCore;
using MVCSite.Models;
using Features.Extensions;
using Features.Enums;
using System.Text;

namespace Features.MariaDB;

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
            new UserIdentityDataModel("admin", Encoding.ASCII.GetString(HashPassword.ComputePasswordHash("admin", 785433)), 785433, Role.Admin)
        );
    }
}