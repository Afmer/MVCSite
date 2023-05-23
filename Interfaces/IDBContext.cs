using Microsoft.EntityFrameworkCore;
using MVCSite.Models;

namespace MVCSite.Interfaces;
public interface IDBContext
{
        public DbSet<UserIdentityDataModel> UserIdentity {get;}
        public DbSet<IdentityTokenDataModel> IdentityTokens {get;}
        public int SaveChanges();
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
}