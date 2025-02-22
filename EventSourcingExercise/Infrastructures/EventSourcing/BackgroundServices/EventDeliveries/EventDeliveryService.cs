using EventSourcingExercise.Infrastructures.EventSourcing.Models;
using EventSourcingExercise.Infrastructures.Projectors;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.EventSourcing.BackgroundServices.EventDeliveries;

public class EventDeliveryService
{
    private readonly EventDeliveryChannel _channel;
    private readonly IClusterClient _clusterClient;
    private readonly IDbContextFactory<EventSourcingDbContext> _dbContextFactory;
    private readonly ILogger<EventDeliveryService> _logger;
    private readonly TimeProvider _timeProvider;

    public EventDeliveryService(
        EventDeliveryChannel channel,
        IClusterClient clusterClient,
        IDbContextFactory<EventSourcingDbContext> dbContextFactory,
        ILogger<EventDeliveryService> logger,
        TimeProvider timeProvider)
    {
        _channel = channel;
        _clusterClient = clusterClient;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task Handle(CancellationToken cancellationToken)
    {
        var streamProvider = _clusterClient.GetStreamProvider("StreamProvider");
        var id = StreamId.Create(ProjectorName.TransactionRecord, string.Empty);
        var stream = streamProvider.GetStream<EventItem>(id);

        await foreach (var package in _channel.Read(cancellationToken))
        {
            await using var paymentDbContext = await _dbContextFactory.CreateDbContextAsync();
            paymentDbContext.AttachRange(package.OutboxEntries);

            try
            {
                foreach (var eventEntry in package.EventEntries)
                {
                    await stream.OnNextAsync(CreateItem(eventEntry, package));
                    var outboxEntry = package.OutboxEntries.Single(t => t.EventId == eventEntry.Id);
                    outboxEntry.Status = EnumOutboxEntryStatus.Delivered;
                    outboxEntry.DeliveredAt = _timeProvider.GetUtcNow().DateTime;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Event Delivery Error. {@EventEntries}", package.EventEntries);
            }

            await paymentDbContext.SaveChangesAsync();
        }
    }

    private static EventItem CreateItem(EventEntry eventEntry, EventDeliveryPackage package)
    {
        return new EventItem
        {
            EventId = eventEntry.Id,
            StreamId = eventEntry.StreamId,
            TenantCode = package.TenantCode,
            Version = eventEntry.Version,
            EventText = eventEntry.EventText,
            EventName = eventEntry.EventName,
            CreatedAt = eventEntry.CreatedAt,
        };
    }
}