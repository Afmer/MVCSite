using Microsoft.EntityFrameworkCore;
using MVCSite.Models;

namespace MVCSite.Interfaces;
public interface IDBContext
{
    public DbSet<UserInformationDataModel> UserInformation {get;}

    public DbSet<IdentityTokenDataModel> IdentityTokens {get;}

    public DbSet<RecipeImageInfoDataModel> RecipeImages {get;}
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    public int SaveChanges();
}