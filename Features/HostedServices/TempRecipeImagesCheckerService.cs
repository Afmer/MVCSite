#pragma warning disable IDE0042
using MVCSite.Features.Configurations;
using MVCSite.Features.Extensions.Constants;
using MVCSite.Interfaces;

namespace MVCSite.Features.HostedServices;
public class TempRecipeImagesCheckerService : IHostedService, IDisposable
{
    private Timer _timer = null!;
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeConfiguration _tempImageLifeTime;
    private readonly TimeConfiguration _timerConfiguration;
    public TempRecipeImagesCheckerService(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        var tempLifeTime = configuration.GetSection(SettingsName.TempImageLifeTimeSettings).Get<TimeConfiguration>();
        if(tempLifeTime != null)
            _tempImageLifeTime = tempLifeTime;
        else 
            throw new Exception("AuthLifeTime didn't set");
        var tempTimer = configuration.GetSection(SettingsName.CheckTempImageTimerlSettings).Get<TimeConfiguration>();
        if(tempTimer != null)
            _timerConfiguration = tempTimer;
        else
            throw new Exception("CheckIdentityTokensTimer didn't set");
    }
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
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IDBManager>();
        var imageService = scope.ServiceProvider.GetRequiredService<IImageService>();
        var result = await db.CheckTempImagesLifeTime(_tempImageLifeTime);
        if (result.Success)
        {
            foreach (var imageName in result.DeletedImages)
                await imageService.Delete(imageName, AppDataFolders.RecipeImages);
        }
    }
}