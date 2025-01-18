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
        while (stoppingToken.IsCancellationRequested == false)
        {
            await _service.Handle(stoppingToken);
        }
    }
}