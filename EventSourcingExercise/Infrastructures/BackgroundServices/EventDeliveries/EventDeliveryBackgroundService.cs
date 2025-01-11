namespace EventSourcingExercise.Infrastructures.BackgroundServices.EventDeliveries;

public class EventDeliveryBackgroundService : BackgroundService
{
    private readonly EventDeliveryService _service;

    public EventDeliveryBackgroundService(EventDeliveryService service)
    {
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() => { _service.Close(); });

        while (stoppingToken.IsCancellationRequested == false)
        {
            await _service.Handle();
        }
    }
}