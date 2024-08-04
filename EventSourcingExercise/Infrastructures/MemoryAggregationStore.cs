using System.Text.Json;
using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.IdGenerators;

namespace EventSourcingExercise.Infrastructures;

public class MemoryAggregationStore : AggregationStoreBase
{
    private static readonly Dictionary<string, long> EntityIdToStreamIdMapping = new();
    private static readonly Dictionary<long, EventStream> EventStreams = new();
    private readonly TimeProvider _timeProvider;
    private readonly INumberIdGenerator _numberIdGenerator;
    private readonly List<EventData> _eventMetadataSet = [];

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
                _eventMetadataSet.Add(eventMetadata);
            }
        }

        return Task.CompletedTask;
    }
}