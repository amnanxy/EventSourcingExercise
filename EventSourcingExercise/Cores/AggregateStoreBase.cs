namespace EventSourcingExercise.Cores;

public abstract class AggregateStoreBase
{
    private readonly HashSet<long> _newAggregateIds = [];
    private readonly Dictionary<long, AggregateRoot> _aggregateRootLookup = new();

    public long Add<TAggregate>(TAggregate aggregateRoot)
        where TAggregate : AggregateRoot
    {
        var streamId = GetStreamId();
        _newAggregateIds.Add(streamId);
        _aggregateRootLookup.Add(streamId, aggregateRoot);
        return streamId;
    }

    protected abstract long GetStreamId();

    public void Update<TAggregate>(TAggregate aggregateRoot)
        where TAggregate : AggregateRoot
    {
        // nothing to do
    }

    public async Task Commit()
    {
        var aggregateItems = _aggregateRootLookup
            .Where(t => t.Value.EventCount > 0)
            .Select(t => new AggregateItem
            {
                IsNewAggregate = _newAggregateIds.Contains(t.Key),
                StreamId = t.Key,
                AggregateRoot = t.Value,
            })
            .ToArray();

        await InternalCommit(aggregateItems);

        _newAggregateIds.Clear();
        foreach (var aggregateItem in aggregateItems)
        {
            aggregateItem.AggregateRoot.ClearEvents();
        }
    }

    protected abstract Task InternalCommit(IReadOnlyList<AggregateItem> aggregateItems);

    public async Task<TAggregate?> Get<TAggregate>(long streamId)
        where TAggregate : AggregateRoot
    {
        if (_aggregateRootLookup.TryGetValue(streamId, out var value))
        {
            return (TAggregate)value;
        }

        var (aggregateObj, events) = await GetStreamEvents(streamId);
        if (aggregateObj == null)
        {
            return null;
        }

        var aggregate = (TAggregate)aggregateObj;
        aggregate.Load(events);
        _aggregateRootLookup[streamId] = aggregate;
        return aggregate;
    }

    protected abstract Task<(object? Aggregate, IReadOnlyList<object> Events)> GetStreamEvents(long streamId);

    protected record AggregateItem
    {
        public bool IsNewAggregate { get; init; }

        public long StreamId { get; init; }

        public required AggregateRoot AggregateRoot { get; init; }
    }
}