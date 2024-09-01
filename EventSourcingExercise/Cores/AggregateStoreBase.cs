namespace EventSourcingExercise.Cores;

public abstract class AggregateStoreBase
{
    private readonly List<string> _newAggregateIds = [];
    private readonly Dictionary<string, IReadOnlyList<object>> _newEvents = new();

    public void Add<TAggregate>(TAggregate aggregateRoot)
        where TAggregate : AggregateRoot
    {
        var events = aggregateRoot.GetEvents();
        aggregateRoot.ClearEvents();

        _newAggregateIds.Add(aggregateRoot.Id);
        _newEvents.Add(aggregateRoot.Id, events);
    }

    public void Update<TAggregate>(TAggregate aggregateRoot)
        where TAggregate : AggregateRoot
    {
        var events = aggregateRoot.GetEvents();
        aggregateRoot.ClearEvents();

        _newEvents[aggregateRoot.Id] = _newEvents.TryGetValue(aggregateRoot.Id, out var value)
            ? [..value, ..events]
            : [..events];
    }

    public async Task Commit()
    {
        var newAggregateIds = _newAggregateIds.ToArray();
        var newEvents = _newEvents.ToDictionary();
        _newAggregateIds.Clear();
        _newEvents.Clear();
        await InternalCommit(newAggregateIds, newEvents);
    }

    protected abstract Task InternalCommit(IReadOnlyList<string> newAggregateIds, IReadOnlyDictionary<string, IReadOnlyList<object>> newEvents);

    public async Task<TAggregate?> Get<TAggregate>(string aggregateId)
        where TAggregate : AggregateRoot, IAggregateCreator<TAggregate>
    {
        var events = await GetStreamEvents(aggregateId);
        if (events.Count == 0)
        {
            return null;
        }

        var aggregate = TAggregate.Create();
        aggregate.Load(events);
        return aggregate;
    }

    protected abstract Task<IReadOnlyList<object>> GetStreamEvents(string aggregateId);
}