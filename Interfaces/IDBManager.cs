using MVCSite.Models;
using MVCSite.Features.Enums;
using MVCSite.Features.Configurations;

namespace MVCSite.Interfaces;
public interface IDBManager
{
       public Task<(LoginStatusCode status, string token)> LoginHandler(string login, string password);
       public Task<(RegisterStatusCode status, string token)> RegisterHandler(RegisterModel model);
       public bool IsHasUser(string login);
       public UserInformationDataModel GetUserInformationFromToken(string token);
       public Task CheckTokensLifeTime(AuthLifeTimeConfiguration config);
       public Task RemoveIdentityToken(string token);
       public Task<(AddRecipeImageStatusCode status, Guid imageId)> AddRecipeImage(RecipeImageInfoDataModel image);
       public IdentityTokenDataModel GetIdentityToken(string token);
       public bool IsHasRecipe(Guid id);
       public Task<AddTempRecipeImageStatusCode> AddTempRecipeImage(TempRecipeImageInfoDataModel image);
       public Task<(MigrateTempImageStatusCode Status, IEnumerable<TempRecipeImageInfoDataModel> ImageForDelete)> CheckAndMigrateTempImages(IEnumerable<Guid> ids, Guid recipeId);
       public Task<bool> AddRecipe(RecipeDataModel recipe);
       public RecipeDataModel GetRecipe(Guid id);
       public Task<(bool Success, Guid[] DeletedImages)> CheckTempImagesLifeTime(TimeConfiguration config);
       public Task<(bool Success, Exception Exception)> ExecuteInTransaction(Func<Task> func);
}