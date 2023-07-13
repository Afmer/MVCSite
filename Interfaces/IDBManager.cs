using MVCSite.Models;
using MVCSite.Features.Enums;
using MVCSite.Features.Configurations;

namespace MVCSite.Interfaces;
public interface IDBManager
{
       public Task<(LoginStatusCode status, string token)> LoginHandler(string login, string password);
       public Task<(RegisterStatusCode status, string token)> RegisterHandler(UserInformationDataModel userDataModel);
       public bool IsHasUser(string login);
       public UserInformationDataModel GetUserInformationFromToken(string token);
       public Task CheckTokensLifeTime(AuthLifeTimeConfiguration config);
       public Task RemoveIdentityToken(string token);
       public Task<(AddRecipeImageStatusCode status, Guid imageId)> AddRecipeImage(RecipeImageInfoDataModel image);
}