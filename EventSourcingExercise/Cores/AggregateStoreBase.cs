namespace EventSourcingExercise.Cores;

public abstract class AggregateStoreBase
{
    private readonly HashSet<string> _newAggregateIds = [];
    private readonly Dictionary<string, AggregateRoot> _aggregateRootLookup = new();

    public void Add<TAggregate>(TAggregate aggregateRoot)
        where TAggregate : AggregateRoot
    {
        _newAggregateIds.Add(aggregateRoot.Id);
        _aggregateRootLookup.Add(aggregateRoot.Id, aggregateRoot);
    }

    public void Update<TAggregate>(TAggregate aggregateRoot)
        where TAggregate : AggregateRoot
    {
        _aggregateRootLookup[aggregateRoot.Id] = aggregateRoot;
    }

    public async Task Commit()
    {
        var aggregateRoots = _aggregateRootLookup
            .Select(t => t.Value)
            .Where(t => t.EventCount > 0);

        var aggregateItems = aggregateRoots
            .Select(aggregateRoot => new AggregateItem
            {
                IsNewAggregate = _newAggregateIds.Contains(aggregateRoot.Id),
                AggregateRoot = aggregateRoot,
                NewEvents = aggregateRoot.GetEvents(),
            })
            .ToArray();

        await InternalCommit(aggregateItems);

        _newAggregateIds.Clear();
        foreach (var aggregateRoot in aggregateRoots)
        {
            aggregateRoot.ClearEvents();
        }
    }

    protected abstract Task InternalCommit(IReadOnlyList<AggregateItem> aggregateItems);

    public async Task<TAggregate?> Get<TAggregate>(string aggregateId)
        where TAggregate : AggregateRoot
    {
        if (_aggregateRootLookup.TryGetValue(aggregateId, out var value))
        {
            return (TAggregate)value;
        }

        var (aggregateObj, events) = await GetStreamEvents(aggregateId);
        if (events.Count == 0)
        {
            return null;
        }

        var aggregate = (TAggregate)aggregateObj;
        aggregate.Load(events);
        _aggregateRootLookup[aggregateId] = aggregate;
        return aggregate;
    }

    protected abstract Task<(object Aggregate, IReadOnlyList<object> Events)> GetStreamEvents(string aggregateId);

    protected record AggregateItem
    {
        public bool IsNewAggregate { get; init; }

        public required AggregateRoot AggregateRoot { get; init; }

        public required IReadOnlyList<object> NewEvents { get; init; }
    }
}