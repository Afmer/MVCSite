using MVCSite.Features.Configurations;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Interfaces;

namespace MVCSite.Features.HostedServices;
public class SearchCacheUpdaterService : IHostedService, IDisposable
{
    private readonly TimeSpan _searchCacheLifeTime;
    private readonly TimeSpan _timerConfiguration;
    private readonly IServiceProvider _serviceProvider;
    private Timer _timer = null!;
    public SearchCacheUpdaterService(IConfiguration configuration, IServiceProvider serviceProvider)
    {
        var tempLifeTime = configuration.GetSection(SettingsName.SearchCacheSettings).Get<TimeConfiguration>();
        if(tempLifeTime == null)
            throw new Exception($"{SettingsName.SearchCacheSettings} is not set");
        var tempTimerConfiguration = configuration.GetSection(SettingsName.SearchCacheTimerSettings).Get<TimeConfiguration>();
        if(tempTimerConfiguration == null)
            throw new Exception($"{SettingsName.SearchCacheTimerSettings} is not set");
        _searchCacheLifeTime = new TimeSpan(tempLifeTime.Days, tempLifeTime.Hours, tempLifeTime.Minutes, tempLifeTime.Seconds);
        _timerConfiguration = new TimeSpan(tempTimerConfiguration.Days, tempTimerConfiguration.Hours, tempTimerConfiguration.Minutes, tempTimerConfiguration.Seconds);
        _serviceProvider = serviceProvider;
    }
    private void ScheduledMethod(object? state)
    {
        using var scope = _serviceProvider.CreateScope();
        var searchCacheUpdater = scope.ServiceProvider.GetRequiredService<ISearchCacheUpdater>();
        searchCacheUpdater.UpdateCache(_searchCacheLifeTime);
    }
    public void Dispose()
    {
        _timer?.Dispose();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        
        _timer = new Timer(ScheduledMethod, null, TimeSpan.Zero, _timerConfiguration); // Расписание вызова метода

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }
}