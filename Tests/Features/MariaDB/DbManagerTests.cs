using Microsoft.EntityFrameworkCore;
using MVCSite.Features.MariaDB;
using MVCSite.Interfaces;
using MVCSite.Models;
using MVCSite.Features.Extensions;
using MVCSite.Features.Enums;
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
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        foreach(var context in _contexts)
            ((MariaDbContext)context).Database.EnsureDeleted();
    }
}