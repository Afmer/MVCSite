using Microsoft.EntityFrameworkCore;
using MVCSite.Features.MariaDB;
using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Extensions;
using MVCSite.Features.Enums;
using MVCSite.Features.Configurations;
namespace Tests;
[TestFixture]
public class DbManagerTests
{
    private readonly string _server = "localhost";
    private readonly string _user = "admin";
    private readonly string _password = "red_alien";
    private readonly Queue<IDBContext> _contexts = new();
    private IDBContext CreateContext(string dbName)
    {
        string connectionString = $"Server={_server};User={_user};Password={_password};Database={dbName}";
        var options = new DbContextOptionsBuilder<MariaDbContext>()
            .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            )
            .Options;
        var dbContext = new MariaDbContext(options);
        _contexts.Enqueue(dbContext);
        return dbContext;
    }
    [SetUp]
    public void Setup()
    {
    }
    [Test]
    public void LoginHandlerTest()
    {
        var dbContext = CreateContext("DBForLoginHAndler");
        var dbManager = new DbManager(dbContext);

        int salt = HashPassword.GenerateSaltForPassword();
        dbContext.UserInformation.Add(new UserInformationDataModel("testUser", HashPassword.ComputePasswordHash("testPassword", salt), salt, Role.User, "test@mail.ru"));
        dbContext.SaveChanges();

        var loginResult = dbManager.LoginHandler("testUser", "testPassword");
        Assert.AreEqual(LoginStatusCode.Success, loginResult.Result.status);
        Assert.True(loginResult.Result.token != null && loginResult.Result.token != "");
        var token = dbContext.IdentityTokens.Find(loginResult.Result.token);
        if(token != null)
            Assert.True(token.IdentityToken == loginResult.Result.token && token.Login == "testUser");
        else
            Assert.Fail("token not found");
        loginResult = dbManager.LoginHandler("testUser13", "testPassword");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = dbManager.LoginHandler("testUser", "testPassword13");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = dbManager.LoginHandler("testUser13", "testPassword13");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = dbManager.LoginHandler("", "");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = dbManager.LoginHandler("testUser", "");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = dbManager.LoginHandler("testUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUser", "testPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPasswordtestPassword");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = dbManager.LoginHandler("", "testPassword");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = dbManager.LoginHandler(null!, null!);
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void RegisterHandlerTest()
    {
        var dbContext = CreateContext("DBForRegisterHandler");
        var dbManager = new DbManager(dbContext);

        int salt = HashPassword.GenerateSaltForPassword();
        dbContext.UserInformation.Add(new UserInformationDataModel("testUser", HashPassword.ComputePasswordHash("testPassword", salt), salt, Role.User, "test@mail.ru"));
        dbContext.SaveChanges();

        var registerResult = dbManager.RegisterHandler(new RegisterModel(){Login = "NewTestUser", Password = "NewTestPassword", Email = "newtest@mail.ru"});
        Assert.AreEqual(RegisterStatusCode.Success, registerResult.Result.status);
        Assert.True(registerResult.Result.token != null && registerResult.Result.token != "");
        registerResult = dbManager.RegisterHandler(new RegisterModel(){Login = "testUser", Password = "NewTestPassword", Email = "newtest@mail.ru"});
        Assert.AreEqual(RegisterStatusCode.LoginExists, registerResult.Result.status);
        Assert.True(registerResult.Result.token == null);
        registerResult = dbManager.RegisterHandler(new RegisterModel(){Login = "NewTestUser2", Password = "NewTestPassword", Email = "test@mail.ru"});
        Assert.AreEqual(RegisterStatusCode.EmailExists, registerResult.Result.status);
        Assert.True(registerResult.Result.token == null);
        registerResult = dbManager.RegisterHandler(new RegisterModel(){Login = "testUser", Password = "NewTestPassword", Email = "test@mail.ru"});
        Assert.AreEqual(RegisterStatusCode.LoginExists, registerResult.Result.status);
        Assert.True(registerResult.Result.token == null);
        registerResult = dbManager.RegisterHandler(new RegisterModel());
        Assert.AreEqual(RegisterStatusCode.Error, registerResult.Result.status);
        Assert.True(registerResult.Result.token == null);
        registerResult = dbManager.RegisterHandler(new RegisterModel(){Login = "NewTestUser3", Password = null, Email = "NewTest3@mail.ru"});
        Assert.AreEqual(RegisterStatusCode.Error, registerResult.Result.status);
        Assert.True(registerResult.Result.token == null);
        registerResult = dbManager.RegisterHandler(new RegisterModel(){Login = "NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3NewTestUser3", Password = null, Email = "NewTest3@mail.ru"});
        Assert.AreEqual(RegisterStatusCode.Error, registerResult.Result.status);
        Assert.True(registerResult.Result.token == null);

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void IsHasUserTest()
    {
        var dbContext = CreateContext("DBForIsHasUser");
        var dbManager = new DbManager(dbContext);

        int salt = HashPassword.GenerateSaltForPassword();
        dbContext.UserInformation.Add(new UserInformationDataModel("testUser", HashPassword.ComputePasswordHash("testPassword", salt), salt, Role.User, "test@mail.ru"));
        dbContext.SaveChanges();

        var isHasUserResult = dbManager.IsHasUser("testUser");
        Assert.True(isHasUserResult);
        isHasUserResult = dbManager.IsHasUser("testUser2");
        Assert.True(!isHasUserResult);
        isHasUserResult = dbManager.IsHasUser("testUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUsertestUser");
        Assert.True(!isHasUserResult);
        isHasUserResult = dbManager.IsHasUser(null!);
        Assert.True(!isHasUserResult);
        isHasUserResult = dbManager.IsHasUser("");
        Assert.True(!isHasUserResult);

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void GetUserInformationFromTokenTest()
    {
        var dbContext = CreateContext("DBForGetUserInformationFromToken");
        var dbManager = new DbManager(dbContext);

        int salt = HashPassword.GenerateSaltForPassword();
        dbContext.UserInformation.Add(new UserInformationDataModel("testUser", HashPassword.ComputePasswordHash("testPassword", salt), salt, Role.User, "test@mail.ru"));
        string token = IdentityToken.Generate();
        dbContext.IdentityTokens.Add(new IdentityTokenDataModel(token, "testUser"));
        dbContext.SaveChanges();

        var getUserResult = dbManager.GetUserInformationFromToken(token);
        Assert.True(getUserResult != null);
        getUserResult = dbManager.GetUserInformationFromToken(IdentityToken.Generate());
        Assert.True(getUserResult == null);
        getUserResult = dbManager.GetUserInformationFromToken(null!);
        Assert.True(getUserResult == null);

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void CheckTokensLifeTimeTest()
    {
        var dbContext = CreateContext("DBForCheckTokensLifeTimeTest");
        var dbManager = new DbManager(dbContext);
        var config = new AuthLifeTimeConfiguration(){Hours = 12};

        var validTokens = new List<IdentityTokenDataModel>();
        var rnd = new Random();
        for(int i = 0; i < 1000; i++)
        {
            int hours = 0;
            while(hours == 0 || hours == config.Hours)
                hours = rnd.Next(48);
            var overTimeSpan = new TimeSpan(hours, 0, 0);
            var token = new IdentityTokenDataModel(IdentityToken.Generate(), "test" + i.ToString()){DateUpdate = DateTime.UtcNow - overTimeSpan};
            dbContext.IdentityTokens.Add(token);
            if(hours <= config.Hours)
                validTokens.Add(token);
        }
        dbContext.SaveChanges();
        dbManager.CheckTokensLifeTime(config).Wait();
        int validNum = validTokens.Count();
        int validExistNum = dbContext.IdentityTokens.Count();
        Assert.True(validTokens.Count == dbContext.IdentityTokens.Count());
        foreach(var token in validTokens)
        {
            var entry = dbContext.IdentityTokens.Find(token.IdentityToken);
            Assert.True(entry != null);
        }

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void RemoveIdentityTokenTest()
    {
        var dbContext = CreateContext("DBForRemoveIdentityTokenTest");
        var dbManager = new DbManager(dbContext);

        List<IdentityTokenDataModel> tokens = new();
        for(int i = 0; i < 1000; i++)
        {
            var token = new IdentityTokenDataModel(IdentityToken.Generate(), "test" + i.ToString());
            dbContext.IdentityTokens.Add(token);
            tokens.Add(token);
        }
        dbContext.SaveChanges();
        int randomIndex = new Random().Next(999);
        var removalToken = tokens[randomIndex];
        tokens.Remove(removalToken);
        dbManager.RemoveIdentityToken(removalToken.IdentityToken).Wait();
        Assert.True(tokens.Count == dbContext.IdentityTokens.Count());
        foreach(var token in tokens)
        {
            var entry = dbContext.IdentityTokens.Find(token.IdentityToken);
            Assert.True(entry != null);
        }
        dbManager.RemoveIdentityToken(IdentityToken.Generate()).Wait();
        Assert.True(tokens.Count == dbContext.IdentityTokens.Count());
        foreach(var token in tokens)
        {
            var entry = dbContext.IdentityTokens.Find(token.IdentityToken);
            Assert.True(entry != null);
        }
        dbManager.RemoveIdentityToken(null!).Wait();
        Assert.True(tokens.Count == dbContext.IdentityTokens.Count());
        foreach(var token in tokens)
        {
            var entry = dbContext.IdentityTokens.Find(token.IdentityToken);
            Assert.True(entry != null);
        }

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void GetIdentityTokenTest()
    {
        var dbContext = CreateContext("DBForRemoveIdentityTokenTest");
        var dbManager = new DbManager(dbContext);

        List<IdentityTokenDataModel> tokens = new();
        for(int i = 0; i < 1000; i++)
        {
            var token = new IdentityTokenDataModel(IdentityToken.Generate(), "test" + i.ToString());
            dbContext.IdentityTokens.Add(token);
            tokens.Add(token);
        }
        dbContext.SaveChanges();
        var rnd = new Random();
        IdentityTokenDataModel result = null!;
        for(int i = 0; i < 2000; i++)
        {
            if(i % 2 == 0)
            {
                var token = tokens[rnd.Next(999)];
                result = dbManager.GetIdentityToken(token.IdentityToken);
                Assert.True(result != null);
            }
            else
            {
                result = dbManager.GetIdentityToken(IdentityToken.Generate());
                Assert.True(result == null);
            }
        }
        result = dbManager.GetIdentityToken(null!);
        Assert.True(result == null);

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void CheckAndMigrateTempImagesTest()
    {
        var dbContext = CreateContext("CheckAndMigrateTempImagesTest");
        var dbManager = new DbManager(dbContext);
        var recipe = ModelFabric.GetRecipeDataModel();
        dbContext.Recipes.Add(recipe);
        dbContext.SaveChanges();
        Queue<Guid> validIds = new();
        Queue<Guid> invalidIds = new();
        var rnd = new Random();
        for(int i = 0; i < 1000; i++)
        {
            var tempImage = ModelFabric.GetTempRecipeImageInfoDataModel();
            tempImage.RecipeId = recipe.Id;
            if(rnd.Next(5) != 4)
                validIds.Enqueue(tempImage.Id);
            else
                invalidIds.Enqueue(tempImage.Id);
            dbContext.TempRecipeImages.Add(tempImage);
        }
        dbContext.SaveChanges();
        var result = dbManager.CheckAndMigrateTempImages(validIds, recipe.Id).Result;
        var tempImages = dbContext.TempRecipeImages.Where(x => x.RecipeId == recipe.Id).ToArray();
        Assert.True(tempImages.Length == 0);
        var images = dbContext.RecipeImages.Where(x => x.RecipeId == recipe.Id).OrderBy(x => x.Id).ToArray();
        var sortedValidIds = validIds.OrderBy(x => x).ToArray();
        if(validIds.Count == images.Length)
        {
            for(int i = 0; i < images.Length; i++)
            {
                if(images[i].Id != sortedValidIds[i])
                    Assert.Fail();
            }
        }
        else
            Assert.Fail();
        var imageForDelete = result.ImageForDelete.OrderBy(x => x.Id).ToArray();
        var sortedInvalidIds = invalidIds.OrderBy(x => x).ToArray();
        if(imageForDelete.Length == invalidIds.Count)
        {
            for(int i = 0; i < invalidIds.Count; i++)
            {
                if(imageForDelete[i].Id != sortedInvalidIds[i])
                    Assert.Fail();
            }
        }
        else
            Assert.Fail();
        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [Test]
    public void CheckTempImagesLifeTimeTest()
    {
        var dbContext = CreateContext("CheckTempImagesLifeTimeTest");
        var dbManager = new DbManager(dbContext);

        var config = new TimeConfiguration(){Hours = 12};

        var validImages = new List<TempRecipeImageInfoDataModel>();
        var invalidImages = new List<TempRecipeImageInfoDataModel>();
        var rnd = new Random();
        for(int i = 0; i < 1000; i++)
        {
            int hours = 0;
            while(hours == 0 || hours == config.Hours)
                hours = rnd.Next(48);
            var overTimeSpan = new TimeSpan(hours, 0, 0);
            var image = new TempRecipeImageInfoDataModel(){Id = Guid.NewGuid(), DateOfCreation = DateTime.UtcNow - overTimeSpan, RecipeId = Guid.NewGuid()};
            dbContext.TempRecipeImages.Add(image);
            if(hours <= config.Hours)
                validImages.Add(image);
            else
                invalidImages.Add(image);
        }
        dbContext.SaveChanges();
        var result = dbManager.CheckTempImagesLifeTime(config).Result;
        Assert.True(result.Success);
        foreach(var image in invalidImages)
        {
            var entry = dbContext.TempRecipeImages.Find(image.Id);
            Assert.True(entry == null);
        }
        foreach(var image in validImages)
        {
            var entry = dbContext.TempRecipeImages.Find(image.Id);
            Assert.True(entry != null);
        }
        var sortedInvalidImages = invalidImages.OrderBy(x => x.Id).ToArray();
        var sortedDeletedImages = result.DeletedImages.OrderBy(x => x).ToArray();
        if(sortedDeletedImages.Length == sortedInvalidImages.Length)
        {
            for(int i = 0; i < sortedDeletedImages.Length; i++)
            {
                Assert.True(sortedInvalidImages[i].Id == sortedDeletedImages[i]);
            }
        }
        else
            Assert.Fail();

        ((MariaDbContext)dbContext).Database.EnsureDeleted();
    }
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        foreach(var context in _contexts)
            ((MariaDbContext)context).Database.EnsureDeleted();
    }
}