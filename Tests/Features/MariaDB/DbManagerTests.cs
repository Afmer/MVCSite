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
    private IDBManager _dbManager = null!;
    private IDBContext _dbContext = null!;
    [SetUp]
    public void Setup()
    {
        string connectionString = "Server=localhost;User=admin;Password=red_alien;Database=DBForTests";
        var options = new DbContextOptionsBuilder<MariaDbContext>()
            .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            )
            .Options;
        _dbContext = new MariaDbContext(options);
        _dbManager = new DbManager(_dbContext);
    }
    [Test]
    public void LoginHandlerTest()
    {
        int salt = HashPassword.GenerateSaltForPassword();
        _dbContext.UserInformation.Add(new UserInformationDataModel("testUser", HashPassword.ComputePasswordHash("testPassword", salt), salt, Role.User, "test@mail.ru"));
        _dbContext.SaveChanges();

        var loginResult = _dbManager.LoginHandler("testUser", "testPassword");
        Assert.AreEqual(LoginStatusCode.Success, loginResult.Result.status);
        Assert.True(loginResult.Result.token != null && loginResult.Result.token != "");
        var token = _dbContext.IdentityTokens.Find(loginResult.Result.token);
        if(token != null)
            Assert.True(token.IdentityToken == loginResult.Result.token && token.Login == "testUser");
        else
            Assert.Fail("token not found");
        loginResult = _dbManager.LoginHandler("testUser13", "testPassword");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = _dbManager.LoginHandler("testUser", "testPassword13");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = _dbManager.LoginHandler("testUser13", "testPassword13");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = _dbManager.LoginHandler("", "");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = _dbManager.LoginHandler("testUser", "");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);
        loginResult = _dbManager.LoginHandler("", "testPassword");
        Assert.AreEqual(LoginStatusCode.LoginOrPasswordError, loginResult.Result.status);
        Assert.True(loginResult.Result.token == null);

        _dbContext.UserInformation.RemoveRange(_dbContext.UserInformation);
        _dbContext.IdentityTokens.RemoveRange(_dbContext.IdentityTokens);
        _dbContext.SaveChanges();
    }
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        ((MariaDbContext)_dbContext).Database.EnsureDeleted();
    }
}