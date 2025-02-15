using EventSourcingExercise.Infrastructures.EventSourcing.BackgroundServices.EventDeliveries;
using MediatR;

namespace EventSourcingExercise.Infrastructures.EventSourcing;

public class EventsCreatedObserver : INotificationHandler<EventsCreated>
{
    private readonly EventDeliveryChannel _eventDeliveryChannel;

    public EventsCreatedObserver(EventDeliveryChannel eventDeliveryChannel)
    {
        _eventDeliveryChannel = eventDeliveryChannel;
    }

    public async Task Handle(EventsCreated notification, CancellationToken cancellationToken)
    {
        await _eventDeliveryChannel.Write(new EventDeliveryPackage
        {
            TenantId = notification.TenantId,
            EventEntries = notification.EventEntries,
            OutboxEntries = notification.OutboxEntries,
        });
    }
}