using System.Text.Json;
using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.IdGenerators;

namespace EventSourcingExercise.Infrastructures;

public class MemoryAggregateStore : AggregateStoreBase
{
    private static readonly Dictionary<string, long> AggregateIdToStreamIdMapping = new();
    private static readonly Dictionary<long, EventStream> EventStreams = new();
    private static readonly List<EventData> EventDataSet = [];
    private readonly TimeProvider _timeProvider;
    private readonly INumberIdGenerator _numberIdGenerator;
    private readonly EventTypeMapper _eventTypeMapper;

    public MemoryAggregateStore(TimeProvider timeProvider, INumberIdGenerator numberIdGenerator, EventTypeMapper eventTypeMapper)
    {
        _timeProvider = timeProvider;
        _numberIdGenerator = numberIdGenerator;
        _eventTypeMapper = eventTypeMapper;
    }

    protected override Task InternalCommit(IReadOnlyList<string> newAggregateIds, IReadOnlyDictionary<string, IReadOnlyList<object>> newEvents)
    {
        foreach (var newAggregateId in newAggregateIds)
        {
            var streamId = _numberIdGenerator.CreateId();
            AggregateIdToStreamIdMapping.Add(newAggregateId, streamId);
            EventStreams.Add(streamId, new EventStream
            {
                StreamId = streamId,
            });
        }

        foreach (var (aggregateId, events) in newEvents)
        {
            var streamId = AggregateIdToStreamIdMapping[aggregateId];
            var eventStream = EventStreams[streamId];
            foreach (var evt in events)
            {
                var eventName = _eventTypeMapper[evt.GetType()];
                var eventMetadata = new EventData(eventStream.StreamId, ++eventStream.Version, JsonSerializer.Serialize(evt), eventName, _timeProvider.GetUtcNow());
                EventDataSet.Add(eventMetadata);
            }
        }

        return Task.CompletedTask;
    }

    protected override Task<IReadOnlyList<object>> GetStreamEvents(string aggregateId)
    {
        if (AggregateIdToStreamIdMapping.TryGetValue(aggregateId, out var streamId))
        {
            var events = EventDataSet.Where(t => t.StreamId == streamId)
                .Select(t => JsonSerializer.Deserialize(t.EventText, _eventTypeMapper[t.EventName])!)
                .ToArray();

            return Task.FromResult<IReadOnlyList<object>>(events);
        }

        return Task.FromResult<IReadOnlyList<object>>([]);
    }
}