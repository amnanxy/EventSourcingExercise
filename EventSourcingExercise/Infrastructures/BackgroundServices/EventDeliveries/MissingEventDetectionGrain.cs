using EventSourcingExercise.Infrastructures.PersistenceModels;
using Microsoft.EntityFrameworkCore;
using Orleans.Timers;

namespace EventSourcingExercise.Infrastructures.BackgroundServices.EventDeliveries;

public sealed class MissingEventDetectionGrain : IGrainBase, IMissingEventDetectionGrain
{
    private readonly EventDeliveryChannel _eventDeliveryChannel;
    private readonly IDbContextFactory<PaymentDbContext> _dbContextFactory;
    private readonly TimeProvider _timeProvider;

    public IGrainContext GrainContext { get; }

    public MissingEventDetectionGrain(
        EventDeliveryChannel eventDeliveryChannel,
        IDbContextFactory<PaymentDbContext> dbContextFactory,
        IGrainContext grainContext,
        ITimerRegistry timerRegistry,
        TimeProvider timeProvider)
    {
        _eventDeliveryChannel = eventDeliveryChannel;
        _dbContextFactory = dbContextFactory;
        _timeProvider = timeProvider;
        timerRegistry.RegisterGrainTimer(
            grainContext,
            callback: Callback,
            state: this,
            options: new GrainTimerCreationOptions
            {
                DueTime = TimeSpan.FromSeconds(0),
                Period = TimeSpan.FromSeconds(10),
            });

        GrainContext = grainContext;
    }

    private async Task Callback(MissingEventDetectionGrain state, CancellationToken cancellationToken)
    {
        await using var paymentDbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var queryable = from outboxEntry in paymentDbContext.OutboxEntries
                        join eventEntry in paymentDbContext.EventEntries on outboxEntry.EventId equals eventEntry.Id
                        where outboxEntry.Status == EnumOutboxEntryStatus.Waiting
                              && outboxEntry.CreatedAt <= _timeProvider.GetUtcNow().AddSeconds(-10).DateTime
                        select new { EventEntry = eventEntry, OutboxEntry = outboxEntry };

        var packageEntries = await queryable.ToArrayAsync(cancellationToken);
        if (packageEntries.Length > 0)
        {
            var eventDeliveryPackage = new EventDeliveryPackage
            {
                EventEntries = packageEntries.Select(t => t.EventEntry).ToArray(),
                OutboxEntries = packageEntries.Select(t => t.OutboxEntry).ToArray(),
            };
            await _eventDeliveryChannel.Write(eventDeliveryPackage);
        }
    }

    public Task Initial()
    {
        return Task.CompletedTask;
    }
}