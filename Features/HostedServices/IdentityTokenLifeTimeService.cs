using MVCSite.Features.Configurations;
using MVCSite.Interfaces;
using MVCSite.Features.Extensions;

namespace MVCSite.Features.HostedServices;
public class IdentityTokenLifeTimeService : IHostedService, IDisposable
{
    private Timer _timer = null!;
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthLifeTimeConfiguration _authLifeTime;
    private readonly CheckIdentityTimerConfiguration _timerConfiguration;
    private bool isFirstLaunch = true;
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
        if(isFirstLaunch)
        {
            isFirstLaunch = false;
            _timer.Change(Timeout.Infinite, 0);
            Thread.Sleep(10000);
            var timer = new TimeSpan(_timerConfiguration.Days, _timerConfiguration.Hours, _timerConfiguration.Minutes, _timerConfiguration.Seconds);
            _timer.Change(TimeSpan.Zero, timer);
        }
        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<IDBManager>();
            await db.CheckTokensLifeTime(_authLifeTime);
        }
    }
    public IdentityTokenLifeTimeService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        var tempLifeTime = configuration.GetSection(Constant.AuthLifeTimeSettings).Get<AuthLifeTimeConfiguration>();
        if(tempLifeTime != null)
            _authLifeTime = tempLifeTime;
        else 
            throw new Exception("AuthLifeTime didn't set");
        var tempTimer = configuration.GetSection(Constant.CheckIdentityTokensSettings).Get<CheckIdentityTimerConfiguration>();
        if(tempTimer != null)
            _timerConfiguration = tempTimer;
        else
            throw new Exception("CheckIdentityTokensTimer didn't set");
    }
}