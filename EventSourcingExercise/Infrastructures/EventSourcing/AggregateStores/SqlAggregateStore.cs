using System.Text.Json;
using EventSourcingExercise.Cores;
using EventSourcingExercise.Infrastructures.EventSourcing.Models;
using EventSourcingExercise.Utilities.IdGenerators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.EventSourcing.AggregateStores;

public class SqlAggregateStore : AggregateStoreBase
{
    private readonly EventSourcingDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly INumberIdGenerator _numberIdGenerator;
    private readonly TypeMapper _typeMapper;
    private readonly IMediator _mediator;
    private readonly ITenantService _tenantService;
    private readonly Dictionary<long, EventStream> _eventStreamLookup = new();

    public SqlAggregateStore(
        EventSourcingDbContext dbContext,
        TimeProvider timeProvider,
        INumberIdGenerator numberIdGenerator,
        TypeMapper typeMapper,
        IMediator mediator,
        ITenantService tenantService)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _numberIdGenerator = numberIdGenerator;
        _typeMapper = typeMapper;
        _mediator = mediator;
        _tenantService = tenantService;
    }

    protected override long GetStreamId()
    {
        return _numberIdGenerator.CreateId();
    }

    protected override async Task InternalCommit(IReadOnlyList<AggregateItem> aggregateItems)
    {
        var tenantCode = await _tenantService.GetTenantCode();
        var dataBuffer = aggregateItems.Aggregate(new DataBuffer(), (buffer, aggregateItem) =>
        {
            var aggregateRoot = aggregateItem.AggregateRoot;
            if (aggregateItem.IsNewAggregate)
            {
                var newEventStream = CreateEventStream(aggregateItem.StreamId, aggregateItem, tenantCode);
                buffer.NewEventStreams.Add(newEventStream);

                _eventStreamLookup[aggregateItem.StreamId] = newEventStream;
            }

            var eventStream = _eventStreamLookup[aggregateItem.StreamId];
            foreach (var evt in aggregateRoot.GetEvents())
            {
                var eventEntry = CreateEventEntry(evt, eventStream);
                var outboxEntry = CreateOutboxEntry(eventEntry);
                buffer.NewEventEntries.Add(eventEntry);
                buffer.NewOutboxEntries.Add(outboxEntry);
            }

            return buffer;
        });

        await _dbContext.EventStreams.AddRangeAsync(dataBuffer.NewEventStreams);
        await _dbContext.EventEntries.AddRangeAsync(dataBuffer.NewEventEntries);
        await _dbContext.OutboxEntries.AddRangeAsync(dataBuffer.NewOutboxEntries);
        await _dbContext.SaveChangesAsync();

        await _mediator.Publish(new EventsCreated
        {
            TenantCode = tenantCode,
            EventEntries = dataBuffer.NewEventEntries,
            OutboxEntries = dataBuffer.NewOutboxEntries,
        });
    }

    private static OutboxEntry CreateOutboxEntry(EventEntry eventEntry)
    {
        return new OutboxEntry
        {
            EventId = eventEntry.Id,
            Status = EnumOutboxEntryStatus.Waiting,
            CreatedAt = eventEntry.CreatedAt,
        };
    }

    private EventEntry CreateEventEntry(object evt, EventStream eventStream)
    {
        var eventName = _typeMapper.GetEventName(evt.GetType());
        return new EventEntry
        {
            Id = _numberIdGenerator.CreateId(),
            StreamId = eventStream.Id,
            Version = ++eventStream.Version,
            EventText = JsonSerializer.Serialize(evt),
            EventName = eventName,
            CreatedAt = _timeProvider.GetUtcNow().DateTime,
        };
    }

    private EventStream CreateEventStream(long streamId, AggregateItem aggregateItem, string tenantCode)
    {
        return new EventStream
        {
            Id = streamId,
            AggregateRootTypeName = _typeMapper.GetAggregateRootName(aggregateItem.AggregateRoot.GetType()),
            TenantCode = tenantCode,
        };
    }

    protected override async Task<(object? Aggregate, IReadOnlyList<object> Events)> GetStreamEvents(long streamId)
    {
        var eventStream = await _dbContext.EventStreams.FindAsync(streamId);

        if (eventStream == null)
        {
            return (null, []);
        }

        var aggregateRootType = _typeMapper.GetAggregateRootType(eventStream.AggregateRootTypeName);
        var aggregateRoot = Activator.CreateInstance(aggregateRootType, nonPublic: true);
        var events = await _dbContext.EventEntries.Where(t => t.StreamId == streamId)
            .Select(t => JsonSerializer.Deserialize(t.EventText, _typeMapper.GetEventType(t.EventName), JsonSerializerOptions.Default)!)
            .ToArrayAsync();

        _eventStreamLookup[streamId] = eventStream;

        return (aggregateRoot, events);
    }

    private class DataBuffer
    {
        public List<EventStream> NewEventStreams { get; } = [];

        public List<EventEntry> NewEventEntries { get; } = [];

        public List<OutboxEntry> NewOutboxEntries { get; } = [];
    }
}