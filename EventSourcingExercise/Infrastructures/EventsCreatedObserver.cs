using EventSourcingExercise.Infrastructures.Projectors;
using MediatR;

namespace EventSourcingExercise.Infrastructures;

public class EventsCreatedObserver : INotificationHandler<EventsCreated>
{
    private readonly IClusterClient _clusterClient;

    public EventsCreatedObserver(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task Handle(EventsCreated notification, CancellationToken cancellationToken)
    {
        var streamProvider = _clusterClient.GetStreamProvider("StreamProvider");
        var id = StreamId.Create(ProjectorName.TransactionRecord, string.Empty);
        var stream = streamProvider.GetStream<EventData>(id);

        foreach (var eventData in notification.EventDataSet)
        {
            await stream.OnNextAsync(eventData);
        }
    }
}