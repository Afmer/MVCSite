using MVCSite.Features.Configurations;
using MVCSite.Features.Enums;
using MVCSite.Features.Extensions;
using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Features.MariaDB;
public class DbManager : IDBManager 
{
    private static Dictionary<string, IdentityTokenDataModel> _identityTokensCache = new();
    private readonly IDBContext _dbContext;
    
    public DbManager(IDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected IdentityTokenDataModel GetIdentityInfoFromeCache(string token)
    {
        lock(_identityTokensCache)
        {
            IdentityTokenDataModel? result;
            if(_identityTokensCache.TryGetValue(token, out result))
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
        Func<IdentityTokenDataModel, bool> predicate = obj =>
        {
            var timeSpan = new TimeSpan(config.Days, config.Hours, config.Minutes, config.Seconds);
            var timeAuthorization = DateTime.UtcNow - obj.DateUpdate;
            return timeAuthorization > timeSpan;
        };
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
        if(removalRecord == null)
        {
            removalRecord = _dbContext.IdentityTokens.Find(token);
        }
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
        if(tokenData == null)
            tokenData = _dbContext.IdentityTokens.Find(token);
        return tokenData!;
    }

    public bool IsHasRecipe(Guid id)
    {
        return _dbContext.Recipes.Any(e => e.Id == id);
    }

    public async Task<AddRecipeImageStatusCode> AddTempRecipeImage(RecipeImageInfoDataModel image)
    {
        try
        {
            _dbContext.TempRecipeImages.Add(image);
            await _dbContext.SaveChangesAsync();
            return AddRecipeImageStatusCode.Success;
        }
        catch
        {
            return AddRecipeImageStatusCode.Error;
        }
    }
}