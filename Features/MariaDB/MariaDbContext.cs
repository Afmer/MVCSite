using Microsoft.EntityFrameworkCore;
using MVCSite.Models;
using MVCSite.Features.Extensions;
using MVCSite.Features.Enums;
using MVCSite.Interfaces;

namespace MVCSite.Features.MariaDB;

public partial class MariaDbContext : Microsoft.EntityFrameworkCore.DbContext, IDBContext
{
    public DbSet<UserInformationDataModel> UserInformation {get; set;} = null!;

    public DbSet<IdentityTokenDataModel> IdentityTokens {get; set;} = null!;

    public DbSet<RecipeImageInfoDataModel> RecipeImages {get; set;} = null!;

    public DbSet<RecipeDataModel> Recipes {get; set;} = null!;
    public MariaDbContext(DbContextOptions<MariaDbContext> options)
        : base(options)
    {
        Database.EnsureCreated();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserInformationDataModel>().HasData(
            new UserInformationDataModel("admin", HashPassword.ComputePasswordHash("admin", 785433), 785433, Role.Admin, "")
        );
        modelBuilder.Entity<UserInformationDataModel>().HasData(
            new UserInformationDataModel("moderator", HashPassword.ComputePasswordHash("moderator", 785433), 785433, Role.Moderator, "")
        );
        modelBuilder.Entity<UserInformationDataModel>().HasData(
            new UserInformationDataModel("user", HashPassword.ComputePasswordHash("user", 785433), 785433, Role.User, "")
        );
        modelBuilder.Entity<RecipeImageInfoDataModel>()
            .HasOne(e => e.Recipe)
            .WithMany()
            .HasForeignKey(e => e.RecipeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}