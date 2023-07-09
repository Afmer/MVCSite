using MVCSite.Features.Configurations;
using MVCSite.Interfaces;
using MVCSite.Models;

namespace MVCSite.Features.HostedServices;
public class IdentityTokenLifeTimeService : IHostedService, IDisposable
{
    private Timer _timer = null!;
    private readonly AuthLifeTimeConfiguration _authLifeTime;
    private readonly CheckIdentityTimerConfiguration _timerConfiguration;
    private IDBContext _db;
    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var timer = new TimeSpan(_timerConfiguration.Days, _timerConfiguration.Hours, _timerConfiguration.Minutes, _timerConfiguration.Seconds);
        _timer = new Timer(ScheduledMethod, null, TimeSpan.Zero, timer); // Расписание вызова метода

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void ScheduledMethod(object? state)
    {
        await _db.CheckTokensLifeTime(_authLifeTime);
    }
    public IdentityTokenLifeTimeService(IDBContext db, IConfiguration configuration)
    {
        _db = db;
        var tempLifeTime = configuration.GetSection("AuthLifeTime").Get<AuthLifeTimeConfiguration>();
        if(tempLifeTime != null)
            _authLifeTime = tempLifeTime;
        else 
            throw new Exception("AuthLifeTime didn't set");
        var tempTimer = configuration.GetSection("CheckIdentityTokensTimer").Get<CheckIdentityTimerConfiguration>();
        if(tempTimer != null)
            _timerConfiguration = tempTimer;
        else
            throw new Exception("CheckIdentityTokensTimer didn't set");
    }
}