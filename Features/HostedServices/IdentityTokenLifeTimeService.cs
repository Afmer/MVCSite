using MVCSite.Features.Configurations;
using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Features.HostedServices;
public class IdentityTokenLifeTimeService : IHostedService, IDisposable
{
    private Timer _timer = null!;
    private readonly AuthLifeTimeConfiguration _authLifeTime;
    private IDBContext _db;
    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(ScheduledMethod, null, TimeSpan.Zero, TimeSpan.FromSeconds(5)); // Расписание вызова метода

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private void ScheduledMethod(object? state)
    {
        Func<IdentityTokenDataModel, bool> predicate = obj =>
        {
            var timeSpan = new TimeSpan(_authLifeTime.Days, _authLifeTime.Hours, _authLifeTime.Minutes, _authLifeTime.Seconds);
            var timeAuthorization = DateTime.UtcNow - obj.DateUpdate;
            return timeAuthorization > timeSpan;
        };
        var timedOutEntries = _db.IdentityTokens.Where(predicate);
        _db.IdentityTokens.RemoveRange(timedOutEntries);
        _db.SaveChanges();
    }
    public IdentityTokenLifeTimeService(IDBContext db, IConfiguration configuration)
    {
        _db = db;
        var tempLifeTime = configuration.GetSection("AuthLifeTime").Get<AuthLifeTimeConfiguration>();
        if(tempLifeTime != null)
            _authLifeTime = tempLifeTime;
        else 
            throw new Exception("AuthLifeTime didn't set");
    }
}