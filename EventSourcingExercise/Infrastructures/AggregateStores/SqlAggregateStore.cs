using System.Text.Json;
using EventSourcingExercise.Cores;
using EventSourcingExercise.Infrastructures.PersistenceModels;
using EventSourcingExercise.Utilities.IdGenerators;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.AggregateStores;

public class SqlAggregateStore : AggregateStoreBase
{
    private readonly PaymentDbContext _dbContext;
    private readonly TimeProvider _timeProvider;
    private readonly INumberIdGenerator _numberIdGenerator;
    private readonly TypeMapper _typeMapper;
    private readonly IMediator _mediator;
    private readonly Dictionary<string, EventStream> _eventStreamLookup = new();

    public SqlAggregateStore(
        PaymentDbContext dbContext,
        TimeProvider timeProvider,
        INumberIdGenerator numberIdGenerator,
        TypeMapper typeMapper,
        IMediator mediator)
    {
        _dbContext = dbContext;
        _timeProvider = timeProvider;
        _numberIdGenerator = numberIdGenerator;
        _typeMapper = typeMapper;
        _mediator = mediator;
    }

    protected override async Task InternalCommit(IReadOnlyList<AggregateItem> aggregateItems)
    {
        var dataBuffer = aggregateItems.Aggregate(new DataBuffer(), (buffer, aggregateItem) =>
        {
            var aggregateRoot = aggregateItem.AggregateRoot;
            if (aggregateItem.IsNewAggregate)
            {
                var newStreamIdMapping = CreateStreamIdMapping(aggregateItem);
                var newEventStream = CreateEventStream(newStreamIdMapping, aggregateItem);
                buffer.NewStreamIdMappings.Add(newStreamIdMapping);
                buffer.NewEventStreams.Add(newEventStream);

                _eventStreamLookup[aggregateRoot.Id] = newEventStream;
            }

            var eventStream = _eventStreamLookup[aggregateRoot.Id];
            foreach (var evt in aggregateRoot.GetEvents())
            {
                var eventEntry = CreateEventEntry(evt, eventStream);
                var outboxEntry = CreateOutboxEntry(eventEntry);
                buffer.NewEventEntries.Add(eventEntry);
                buffer.NewOutboxEntries.Add(outboxEntry);
            }

            return buffer;
        });

        await _dbContext.StreamIdMappings.AddRangeAsync(dataBuffer.NewStreamIdMappings);
        await _dbContext.EventStreams.AddRangeAsync(dataBuffer.NewEventStreams);
        await _dbContext.EventEntries.AddRangeAsync(dataBuffer.NewEventEntries);
        await _dbContext.OutboxEntries.AddRangeAsync(dataBuffer.NewOutboxEntries);
        await _dbContext.SaveChangesAsync();

        await _mediator.Publish(new EventsCreated
        {
            EventEntries = dataBuffer.NewEventEntries,
            OutboxEntries = dataBuffer.NewOutboxEntries,
        });
    }

    private static OutboxEntry CreateOutboxEntry(EventEntry eventEntry)
    {
        var outboxEntry = new OutboxEntry
        {
            EventId = eventEntry.Id,
            Status = EnumOutboxEntryStatus.Waiting,
            CreatedAt = eventEntry.CreatedAt,
        };
        return outboxEntry;
    }

    private EventEntry CreateEventEntry(object evt, EventStream eventStream)
    {
        var eventName = _typeMapper.GetEventName(evt.GetType());
        var eventEntry = new EventEntry
        {
            Id = _numberIdGenerator.CreateId(),
            StreamId = eventStream.Id,
            Version = ++eventStream.Version,
            EventText = JsonSerializer.Serialize(evt),
            EventName = eventName,
            CreatedAt = _timeProvider.GetUtcNow(),
        };
        return eventEntry;
    }

    private EventStream CreateEventStream(StreamIdMapping newStreamIdMapping, AggregateItem aggregateItem)
    {
        var newEventStream = new EventStream
        {
            Id = newStreamIdMapping.StreamId,
            AggregateRootTypeName = _typeMapper.GetAggregateRootName(aggregateItem.AggregateRoot.GetType()),
        };
        return newEventStream;
    }

    private StreamIdMapping CreateStreamIdMapping(AggregateItem aggregateItem)
    {
        var streamId = _numberIdGenerator.CreateId();
        var newStreamIdMapping = new StreamIdMapping
        {
            StreamId = streamId,
            AggregateId = aggregateItem.AggregateRoot.Id,
        };
        return newStreamIdMapping;
    }

    protected override async Task<(object? Aggregate, IReadOnlyList<object> Events)> GetStreamEvents(string aggregateId)
    {
        var streamIdMapping = await _dbContext.StreamIdMappings.FindAsync(aggregateId);
        if (streamIdMapping == null)
        {
            return (null, []);
        }

        var eventStream = (await _dbContext.EventStreams.FindAsync(streamIdMapping.StreamId))!;
        var aggregateRootType = _typeMapper.GetAggregateRootType(eventStream.AggregateRootTypeName);
        var aggregateRoot = Activator.CreateInstance(aggregateRootType, nonPublic: true);
        var events = await _dbContext.EventEntries.Where(t => t.StreamId == streamIdMapping.StreamId)
            .Select(t => JsonSerializer.Deserialize(t.EventText, _typeMapper.GetEventType(t.EventName), JsonSerializerOptions.Default)!)
            .ToArrayAsync();

        _eventStreamLookup[aggregateId] = eventStream;

        return (aggregateRoot, events);
    }

    private class DataBuffer
    {
        public List<EventStream> NewEventStreams { get; } = [];

        public List<StreamIdMapping> NewStreamIdMappings { get; } = [];

        public List<EventEntry> NewEventEntries { get; } = [];

        public List<OutboxEntry> NewOutboxEntries { get; } = [];
    }
}