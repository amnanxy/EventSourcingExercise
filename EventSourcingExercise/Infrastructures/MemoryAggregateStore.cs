using System.Text.Json;
using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.IdGenerators;
using MediatR;

namespace EventSourcingExercise.Infrastructures;

public class MemoryAggregateStore : AggregateStoreBase
{
    private static readonly Dictionary<string, long> AggregateIdToStreamIdMapping = new();
    private static readonly Dictionary<long, EventStream> EventStreams = new();
    private static readonly List<EventData> EventDataSet = [];
    private readonly TimeProvider _timeProvider;
    private readonly INumberIdGenerator _numberIdGenerator;
    private readonly TypeMapper _typeMapper;
    private readonly IMediator _mediator;

    public MemoryAggregateStore(TimeProvider timeProvider, INumberIdGenerator numberIdGenerator, TypeMapper typeMapper, IMediator mediator)
    {
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
            AggregateIdToStreamIdMapping.Add(aggregateItem.AggregateRoot.Id, streamId);
            EventStreams.Add(streamId, new EventStream
            {
                StreamId = streamId,
                AggregateRootTypeName = _typeMapper.GetAggregateRootName(aggregateItem.AggregateRoot.GetType()),
            });
        }

        var newEventDataSet = new List<EventData>();
        foreach (var aggregateItem in aggregateItems)
        {
            var streamId = AggregateIdToStreamIdMapping[aggregateItem.AggregateRoot.Id];
            var eventStream = EventStreams[streamId];
            foreach (var evt in aggregateItem.NewEvents)
            {
                var eventName = _typeMapper.GetEventName(evt.GetType());
                var eventData = new EventData
                {
                    StreamId = eventStream.StreamId,
                    Version = ++eventStream.Version,
                    EventText = JsonSerializer.Serialize(evt),
                    EventName = eventName,
                    CreatedAt = _timeProvider.GetUtcNow(),
                };
                newEventDataSet.Add(eventData);
            }
        }

        EventDataSet.AddRange(newEventDataSet);

        await _mediator.Publish(new EventsCreated
        {
            EventDataSet = newEventDataSet,
        });
    }

    protected override Task<(object Aggregate, IReadOnlyList<object> Events)> GetStreamEvents(string aggregateId)
    {
        if (AggregateIdToStreamIdMapping.TryGetValue(aggregateId, out var streamId))
        {
            var eventStream = EventStreams[streamId];
            var aggregateRootType = _typeMapper.GetAggregateRootType(eventStream.AggregateRootTypeName);
            var aggregateRoot = Activator.CreateInstance(aggregateRootType, nonPublic: true);

            var events = EventDataSet.Where(t => t.StreamId == streamId)
                .Select(t => JsonSerializer.Deserialize(t.EventText, _typeMapper.GetEventType(t.EventName))!)
                .ToArray();

            return Task.FromResult<(object, IReadOnlyList<object>)>((aggregateRoot, events)!);
        }

        return Task.FromResult<(object, IReadOnlyList<object>)>((null, [])!);
    }
}