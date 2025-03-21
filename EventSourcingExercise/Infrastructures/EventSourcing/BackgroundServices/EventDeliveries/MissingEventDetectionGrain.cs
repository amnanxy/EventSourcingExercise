﻿using EventSourcingExercise.Infrastructures.EventSourcing.Models;
using Microsoft.EntityFrameworkCore;
using Orleans.Timers;

namespace EventSourcingExercise.Infrastructures.EventSourcing.BackgroundServices.EventDeliveries;

public sealed class MissingEventDetectionGrain : IGrainBase, IMissingEventDetectionGrain
{
    private readonly EventDeliveryChannel _eventDeliveryChannel;
    private readonly IDbContextFactory<EventSourcingReadOnlyDbContext> _dbContextFactory;
    private readonly TimeProvider _timeProvider;

    public IGrainContext GrainContext { get; }

    public MissingEventDetectionGrain(
        EventDeliveryChannel eventDeliveryChannel,
        IDbContextFactory<EventSourcingReadOnlyDbContext> dbContextFactory,
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

        var missingEntries = await queryable.ToArrayAsync(cancellationToken);
        var missingGroups = missingEntries.GroupBy(t => t.EventEntry.StreamId)
            .ToArray();

        if (missingGroups.Length > 0)
        {
            var streamIds = missingGroups
                .Select(t => t.Key);

            var tenantCodeLookup = await paymentDbContext.EventStreams
                .Where(t => streamIds.Contains(t.Id))
                .Select(t => new { t.Id, t.TenantCode })
                .ToDictionaryAsync(t => t.Id, t => t.TenantCode, cancellationToken);

            foreach (var missingGroup in missingGroups)
            {
                var eventDeliveryPackage = new EventDeliveryPackage
                {
                    TenantCode = tenantCodeLookup[missingGroup.Key],
                    EventEntries = missingGroup.Select(t => t.EventEntry).ToArray(),
                    OutboxEntries = missingGroup.Select(t => t.OutboxEntry).ToArray(),
                };
                await _eventDeliveryChannel.Write(eventDeliveryPackage);
            }
        }
    }

    public Task Initial()
    {
        return Task.CompletedTask;
    }
}