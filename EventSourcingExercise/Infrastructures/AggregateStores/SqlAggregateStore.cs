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
        foreach (var aggregateItem in aggregateItems.Where(t => t.IsNewAggregate))
        {
            var streamId = _numberIdGenerator.CreateId();
            await _dbContext.StreamIdMappings.AddAsync(new StreamIdMapping
            {
                StreamId = streamId,
                AggregateId = aggregateItem.AggregateRoot.Id,
            });
            await _dbContext.EventStreams.AddAsync(new EventStream
            {
                Id = streamId,
                AggregateRootTypeName = _typeMapper.GetAggregateRootName(aggregateItem.AggregateRoot.GetType()),
            });
        }

        var newEventDataSet = new List<EventEntry>();
        foreach (var aggregateItem in aggregateItems)
        {
            var streamIdMapping = (await _dbContext.StreamIdMappings.FindAsync(aggregateItem.AggregateRoot.Id))!;
            var eventStream = (await _dbContext.EventStreams.FindAsync(streamIdMapping.StreamId))!;
            foreach (var evt in aggregateItem.NewEvents)
            {
                var eventName = _typeMapper.GetEventName(evt.GetType());
                var eventData = new EventEntry
                {
                    Id = _numberIdGenerator.CreateId(),
                    StreamId = eventStream.Id,
                    Version = ++eventStream.Version,
                    EventText = JsonSerializer.Serialize(evt),
                    EventName = eventName,
                    CreatedAt = _timeProvider.GetUtcNow(),
                };
                newEventDataSet.Add(eventData);
            }
        }

        var outboxEntries = newEventDataSet.Select(t => new OutboxEntry
        {
            EventId = t.Id,
            Status = EnumOutboxEntryStatus.Waiting,
            CreatedAt = t.CreatedAt,
        }).ToArray();

        await _dbContext.EventEntries.AddRangeAsync(newEventDataSet);
        await _dbContext.OutboxEntries.AddRangeAsync(outboxEntries);

        await _dbContext.SaveChangesAsync();

        await _mediator.Publish(new EventsCreated
        {
            EventDataSet = newEventDataSet,
            OutboxEntries = outboxEntries,
        });
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

        return (aggregateRoot, events);
    }
}