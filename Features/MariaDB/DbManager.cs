using MVCSite.Features.Configurations;
using MVCSite.Features.Enums;
using MVCSite.Features.Extensions;
using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Features.MariaDB;
public class DbManager : IDBManager 
{
    private static readonly Dictionary<string, IdentityTokenDataModel> _identityTokensCache = new();
    private readonly IDBContext _dbContext;
    
    public DbManager(IDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected IdentityTokenDataModel GetIdentityInfoFromeCache(string token)
    {
        lock(_identityTokensCache)
        {
            if (_identityTokensCache.TryGetValue(token, out IdentityTokenDataModel? result))
                return result;
            else
                return null!;
        }
    }

    protected void DeleteIdentityFromCache(string token)
    {
        lock(_identityTokensCache)
        {
            if(_identityTokensCache.ContainsKey(token))
                _identityTokensCache.Remove(token);
        }
    }

    protected void SetIdentityInCache(IdentityTokenDataModel obj)
    {
        lock(_identityTokensCache)
        {
            if(_identityTokensCache.ContainsKey(obj.IdentityToken))
                _identityTokensCache[obj.IdentityToken] = obj;
            else
                _identityTokensCache.Add(obj.IdentityToken, obj);
        }
    }

       protected void DeleteIdentityWithPredicateFromCache(Func<IdentityTokenDataModel, bool> predicate)
    {
        lock(_identityTokensCache)
        {
            Queue<string> tokensForRemoval = new();
            foreach(var item in _identityTokensCache)
            {
                if(predicate(item.Value))
                    tokensForRemoval.Enqueue(item.Key);
            }
            foreach(var token in tokensForRemoval)
                _identityTokensCache.Remove(token);
        }
    }

    public async Task<(LoginStatusCode status, string token)> LoginHandler(string login, string password)
    {
        try
        {
            var user = _dbContext.UserInformation.Find(login);
            if (user is null) return (LoginStatusCode.LoginOrPasswordError, null!);
            if(!HashPassword.IsPasswordValid(password, user.Salt, user.PasswordHash)) return (LoginStatusCode.LoginOrPasswordError, null!);
            var token = IdentityToken.Generate();
            var identityObj = new Models.IdentityTokenDataModel(token, login){DateUpdate = DateTime.UtcNow};
            _dbContext.IdentityTokens.Add(identityObj);
            await _dbContext.SaveChangesAsync();
            SetIdentityInCache(identityObj);
            return (LoginStatusCode.Success, token);
        }
        catch
        {
            return (LoginStatusCode.Error, "");
        }
    }

    public async Task<(RegisterStatusCode status, string token)> RegisterHandler(RegisterModel model)
    {
        try
        {
            if(model == null || model.Login == null || model.Password == null || model.Email == null)
                return (RegisterStatusCode.Error, null!);
            var loginResult = _dbContext.UserInformation.Find(model.Login);
            if(loginResult != null)
                return (RegisterStatusCode.LoginExists, null!);
            bool isEmailExists = _dbContext.UserInformation.Any(e => e.Email == model.Email);
            if(isEmailExists)
                return (RegisterStatusCode.EmailExists, null!);
            var salt = HashPassword.GenerateSaltForPassword();
            var hash = HashPassword.ComputePasswordHash(model.Password, salt);
            var userDataModel = new UserInformationDataModel(model.Login, hash, salt, Role.User, model.Email);
            _dbContext.UserInformation.Add(userDataModel);
            var token = IdentityToken.Generate();
            var identityObj = new Models.IdentityTokenDataModel(token, userDataModel.Login){DateUpdate = DateTime.UtcNow};
            _dbContext.IdentityTokens.Add(identityObj);
            await _dbContext.SaveChangesAsync();
            SetIdentityInCache(identityObj);
            return (RegisterStatusCode.Success, token);
        }
        catch
        {
            return (RegisterStatusCode.Error, null!);
        }
    }

    public bool IsHasUser(string login)
    {
        var user = _dbContext.UserInformation.Find(login);
        return user != null;
    }

    public UserInformationDataModel GetUserInformationFromToken(string token)
    {
        if(token == null)
            return null!;
        var tokenRecord = GetIdentityInfoFromeCache(token);
        if(tokenRecord == null)
        {
            tokenRecord = _dbContext.IdentityTokens.Find(token);
            if(tokenRecord != null)
                SetIdentityInCache(tokenRecord);
        }
        if(tokenRecord != null)
        {
            return _dbContext.UserInformation.Find(tokenRecord.Login)!;
        }
        return null!;
    }

    public async Task CheckTokensLifeTime(AuthLifeTimeConfiguration config)
    {
        bool predicate(IdentityTokenDataModel obj)
        {
            var timeSpan = new TimeSpan(config.Days, config.Hours, config.Minutes, config.Seconds);
            var timeAuthorization = DateTime.UtcNow - obj.DateUpdate;
            return timeAuthorization > timeSpan;
        }
        DeleteIdentityWithPredicateFromCache(predicate);
        var timedOutEntries = _dbContext.IdentityTokens.Where(predicate);
        _dbContext.IdentityTokens.RemoveRange(timedOutEntries);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveIdentityToken(string token)
    {
        if(token == null)
            return;
        var removalRecord = GetIdentityInfoFromeCache(token);
        removalRecord ??= _dbContext.IdentityTokens.Find(token); //если removal = null, то removal присваивается значение
        if(removalRecord != null)
        {
            DeleteIdentityFromCache(token);
            try
            {
                _dbContext.IdentityTokens.Remove(removalRecord);
                await _dbContext.SaveChangesAsync();
            }
            catch{}
        }
    }
    public async Task<(AddRecipeImageStatusCode status, Guid imageId)> AddRecipeImage(RecipeImageInfoDataModel image)
    {
        try
        {
            _dbContext.RecipeImages.Add(image);
            var result = image.Id;
            await _dbContext.SaveChangesAsync();
            return (AddRecipeImageStatusCode.Success, result);
        }
        catch
        {
            return (AddRecipeImageStatusCode.Error, Guid.Empty);
        }
    }

    public IdentityTokenDataModel GetIdentityToken(string token)
    {
        if(token == null)
            return null!;
        var tokenData = GetIdentityInfoFromeCache(token);
        tokenData ??= _dbContext.IdentityTokens.Find(token);
        return tokenData!;
    }

    public bool IsHasRecipe(Guid id)
    {
        return _dbContext.Recipes.Any(e => e.Id == id);
    }

    public async Task<AddTempRecipeImageStatusCode> AddTempRecipeImage(TempRecipeImageInfoDataModel image)
    {
        try
        {
            _dbContext.TempRecipeImages.Add(image);
            await _dbContext.SaveChangesAsync();
            return AddTempRecipeImageStatusCode.Success;
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            return AddTempRecipeImageStatusCode.Error;
        }
    }
    public async Task<(MigrateTempImageStatusCode Status, IEnumerable<TempRecipeImageInfoDataModel> ImageForDelete)> CheckAndMigrateTempImages(IEnumerable<Guid> ids, Guid recipeId)
    {
        var imagesForDelete = new Queue<TempRecipeImageInfoDataModel>();
        var sortedIds = ids.OrderBy(x => x);
        var idsEnumerator = sortedIds.GetEnumerator();
        var tempImages = _dbContext.TempRecipeImages.Where(x => x.RecipeId == recipeId).OrderBy(x => x.Id).ToList();
        var tempImagesEnumerator = tempImages.GetEnumerator();

        static RecipeImageInfoDataModel ConvertEntry(TempRecipeImageInfoDataModel entry)
        {
            var result = new RecipeImageInfoDataModel
            {
                DateOfCreation = entry.DateOfCreation,
                Id = entry.Id,
                RecipeId = entry.RecipeId
            };
            return result;
        }
        while (idsEnumerator.MoveNext() && tempImagesEnumerator.MoveNext())
        {
            var currentGuid = idsEnumerator.Current;
            var currentTempImage = tempImagesEnumerator.Current;
            if(currentGuid == currentTempImage.Id)
            {
                _dbContext.RecipeImages.Add(ConvertEntry(currentTempImage));
            }
            else
            {
                while(currentGuid != currentTempImage.Id)
                {
                    imagesForDelete.Enqueue(currentTempImage);
                    if(tempImagesEnumerator.MoveNext())
                        currentTempImage = tempImagesEnumerator.Current;
                    else
                        break;

                }
                if(currentGuid == currentTempImage.Id)
                    _dbContext.RecipeImages.Add(ConvertEntry(currentTempImage));
            }
        }
        while(tempImagesEnumerator.MoveNext())
            imagesForDelete.Enqueue(tempImagesEnumerator.Current);
        var deletedEntities = _dbContext.TempRecipeImages.Where(x => x.RecipeId == recipeId).ToArray();
        if(deletedEntities.Any())
            _dbContext.TempRecipeImages.RemoveRange(deletedEntities);
        try
        {
            await _dbContext.SaveChangesAsync();
            return (MigrateTempImageStatusCode.Success, imagesForDelete);
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return (MigrateTempImageStatusCode.Error, imagesForDelete);
        }
    }

    public async Task<bool> AddRecipe(RecipeDataModel recipe)
    {
        try
        {
            _dbContext.Recipes.Add(recipe);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
    }
    public RecipeDataModel GetRecipe(Guid id)
    {
        var recipe = _dbContext.Recipes.Find(id);
        return recipe!;
    }

    public async Task<(bool Success, Guid[] DeletedImages)> CheckTempImagesLifeTime(TimeConfiguration config)
    {
        bool predicate(TempRecipeImageInfoDataModel obj)
        {
            var timeSpan = new TimeSpan(config.Days, config.Hours, config.Minutes, config.Seconds);
            var timeAuthorization = DateTime.UtcNow - obj.DateOfCreation;
            return timeAuthorization > timeSpan;
        }
        try
        {
            var timedOutEntries = _dbContext.TempRecipeImages.Where(predicate);
            var deletedImages = timedOutEntries.Select(x => x.Id).ToArray();
            _dbContext.TempRecipeImages.RemoveRange(timedOutEntries);
            await _dbContext.SaveChangesAsync();
            return (true, deletedImages);
        }
        catch
        {
            return (false, null!);
        }
    }
    public async Task<(bool Success, Exception Exception)> ExecuteInTransaction(Func<Task> func)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            await func.Invoke();
            await transaction.CommitAsync();
            return (true, null!);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            return (false, e);
        }
    }
    public async Task<(bool Success, Guid[] DeletedImages)> DeleteAllTempRecipeImages(Guid recipeId)
    {
        try
        {
            var imagesForDelete = _dbContext.TempRecipeImages.Where(x => x.RecipeId == recipeId);
            var deletedImages = imagesForDelete.Select(x => x.Id).ToArray();
            _dbContext.TempRecipeImages.RemoveRange(imagesForDelete);
            await _dbContext.SaveChangesAsync();
            return (true, deletedImages);
        }
        catch
        {
            return (false, null!);
        }
    }
    public (Guid RecipeId, Guid imageId)[] GetRecipeLabelImageIds(IEnumerable<Guid> recipeIds)
    {
        var recipeIdsToCheck = new HashSet<Guid>(recipeIds);
        var result = _dbContext.Recipes.Where(x => recipeIdsToCheck.Contains(x.Id))
            .Select<RecipeDataModel, (Guid RecipeId, Guid imageId)>(x => new(x.Id, x.LabelImage))
            .ToArray();
        return result;
    }
}