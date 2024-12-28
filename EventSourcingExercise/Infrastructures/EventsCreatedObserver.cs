using EventSourcingExercise.Infrastructures.PersistenceModels;
using EventSourcingExercise.Infrastructures.Projectors;
using MediatR;

namespace EventSourcingExercise.Infrastructures;

public class EventsCreatedObserver : INotificationHandler<EventsCreated>
{
    private readonly IClusterClient _clusterClient;
    private readonly PaymentDbContext _dbContext;
    private readonly TimeProvider _timeProvider;

    public EventsCreatedObserver(IClusterClient clusterClient, PaymentDbContext dbContext, TimeProvider timeProvider)
    {
        _clusterClient = clusterClient;
        _dbContext = dbContext;
        _timeProvider = timeProvider;
    }

    public async Task Handle(EventsCreated notification, CancellationToken cancellationToken)
    {
        _dbContext.OutboxEntries.AttachRange(notification.OutboxEntries);
        var streamProvider = _clusterClient.GetStreamProvider("StreamProvider");
        var id = StreamId.Create(ProjectorName.TransactionRecord, string.Empty);
        var stream = streamProvider.GetStream<EventEntry>(id);

        foreach (var eventData in notification.EventDataSet)
        {
            await stream.OnNextAsync(eventData);
            var outboxEntry = notification.OutboxEntries.Single(t => t.EventId == eventData.Id);
            outboxEntry.Status = EnumOutboxEntryStatus.Delivered;
            outboxEntry.DeliveredAt = _timeProvider.GetUtcNow();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}