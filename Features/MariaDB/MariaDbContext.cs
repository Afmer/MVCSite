using Microsoft.EntityFrameworkCore;
using MVCSite.Models;
using MVCSite.Features.Extensions;
using MVCSite.Features.Enums;
using MVCSite.Interfaces;
using MVCSite.Features.Configurations;

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

    public async Task<(LoginStatusCode status, string token)> LoginHandler(string login, string password)
    {
        try
        {
            var user = UserInformation.Find(login);
            if (user is null) return (LoginStatusCode.LoginOrPasswordError, "");
            if(!HashPassword.IsPasswordValid(password, user.Salt, user.PasswordHash)) return (LoginStatusCode.LoginOrPasswordError, "");
            var token = IdentityToken.Generate();
            IdentityTokens.Add(new Models.IdentityTokenDataModel(token, login){DateUpdate = DateTime.UtcNow});
            await SaveChangesAsync();
            return (LoginStatusCode.Success, token);
        }
        catch
        {
            return (LoginStatusCode.Error, "");
        }
    }

    public async Task<(RegisterStatusCode status, string token)> RegisterHandler(UserInformationDataModel userDataModel)
    {
        try
        {
            UserInformation.Add(userDataModel);
            var token = IdentityToken.Generate();
            IdentityTokens.Add(new Models.IdentityTokenDataModel(token, userDataModel.Login){DateUpdate = DateTime.UtcNow});
            await SaveChangesAsync();
            return (RegisterStatusCode.Success, token);
        }
        catch
        {
            return (RegisterStatusCode.Error, "");
        }
    }

    public bool IsHasUser(string login)
    {
        var user = UserInformation.Find(login);
        return user != null;
    }

    public UserInformationDataModel GetUserInformationFromToken(string token)
    {
        var tokenRecord = IdentityTokens.Find(token);
        if(tokenRecord != null)
        {
            return UserInformation.Find(tokenRecord.Login)!;
        }
        return null!;
    }

    public async Task CheckTokensLifeTime(AuthLifeTimeConfiguration config)
    {
        Func<IdentityTokenDataModel, bool> predicate = obj =>
        {
            var timeSpan = new TimeSpan(config.Days, config.Hours, config.Minutes, config.Seconds);
            var timeAuthorization = DateTime.UtcNow - obj.DateUpdate;
            return timeAuthorization > timeSpan;
        };
        var timedOutEntries = IdentityTokens.Where(predicate);
        IdentityTokens.RemoveRange(timedOutEntries);
        await SaveChangesAsync();
    }

    public async Task RemoveIdentityToken(string token)
    {
        var removalRecord = IdentityTokens.Find(token);
        if(removalRecord != null)
        {
            IdentityTokens.Remove(removalRecord);
            await SaveChangesAsync();
        }
    }
    public async Task<(AddRecipeImageStatusCode status, Guid imageId)> AddRecipeImage(RecipeImageInfoDataModel image)
    {
        try
        {
            RecipeImages.Add(image);
            var result = image.Id;
            await SaveChangesAsync();
            return (AddRecipeImageStatusCode.Success, result);
        }
        catch
        {
            return (AddRecipeImageStatusCode.Error, Guid.Empty);
        }
    }
}