using Microsoft.EntityFrameworkCore;
using MVCSite.Models;

namespace MVCSite.Interfaces;
public interface IDBContext
{
        public DbSet<UserIdentityDataModel> UserIdentity {get; set;}
}