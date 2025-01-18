namespace EventSourcingExercise.Infrastructures.BackgroundServices.EventDeliveries;

public class MissingEventDetectionBackgroundService : BackgroundService
{
    private readonly IClusterClient _clusterClient;

    public MissingEventDetectionBackgroundService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            var grain = _clusterClient.GetGrain<IMissingEventDetectionGrain>(string.Empty);
            await grain.Initial();
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}