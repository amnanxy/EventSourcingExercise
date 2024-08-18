using System.Text.Json;
using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.IdGenerators;

namespace EventSourcingExercise.Infrastructures;

public class MemoryAggregationStore : AggregationStoreBase
{
    private static readonly Dictionary<string, long> EntityIdToStreamIdMapping = new();
    private static readonly Dictionary<long, EventStream> EventStreams = new();
    private static readonly List<EventData> EventDataSet = [];
    private readonly TimeProvider _timeProvider;
    private readonly INumberIdGenerator _numberIdGenerator;

    public MemoryAggregationStore(TimeProvider timeProvider, INumberIdGenerator numberIdGenerator)
    {
        _timeProvider = timeProvider;
        _numberIdGenerator = numberIdGenerator;
    }

    protected override Task InternalCommit(IReadOnlyList<string> newEntityIds, IReadOnlyDictionary<string, IReadOnlyList<object>> newEvents)
    {
        foreach (var newEntityId in newEntityIds)
        {
            var streamId = _numberIdGenerator.CreateId();
            EntityIdToStreamIdMapping.Add(newEntityId, streamId);
            EventStreams.Add(streamId, new EventStream
            {
                StreamId = streamId,
            });
        }

        foreach (var (entityId, events) in newEvents)
        {
            var streamId = EntityIdToStreamIdMapping[entityId];
            var eventStream = EventStreams[streamId];
            foreach (var evt in events)
            {
                var eventMetadata = new EventData(eventStream.StreamId, ++eventStream.Version, JsonSerializer.Serialize(evt), evt.GetType(), _timeProvider.GetUtcNow());
                EventDataSet.Add(eventMetadata);
            }
        }

        return Task.CompletedTask;
    }

    protected override Task<IReadOnlyList<object>> GetStreamEvents(string entityId)
    {
        if (EntityIdToStreamIdMapping.TryGetValue(entityId, out var streamId))
        {
            var events = EventDataSet.Where(t => t.StreamId == streamId)
                .Select(t => JsonSerializer.Deserialize(t.EventText, t.EventType)!)
                .ToArray();

            return Task.FromResult<IReadOnlyList<object>>(events);
        }

        return Task.FromResult<IReadOnlyList<object>>(Array.Empty<object>());
    }
}