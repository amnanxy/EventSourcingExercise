namespace EventSourcingExercise.Cores;

public abstract class AggregationStoreBase
{
    private readonly List<string> _newEntityIds = [];
    private readonly Dictionary<string, IReadOnlyList<object>> _newEvents = new();

    public void Add<TEntity>(TEntity aggregateRoot)
        where TEntity : AggregateRoot
    {
        var events = aggregateRoot.GetEvents();
        aggregateRoot.ClearEvents();

        _newEntityIds.Add(aggregateRoot.Id);
        _newEvents.Add(aggregateRoot.Id, events);
    }

    public void Update<TEntity>(TEntity aggregateRoot)
        where TEntity : AggregateRoot
    {
        var events = aggregateRoot.GetEvents();
        aggregateRoot.ClearEvents();

        _newEvents[aggregateRoot.Id] = _newEvents.TryGetValue(aggregateRoot.Id, out var value)
            ? [..value, ..events]
            : [..events];
    }

    public async Task Commit()
    {
        var newEntityIds = _newEntityIds.ToArray();
        var newEvents = _newEvents.ToDictionary();
        _newEntityIds.Clear();
        _newEvents.Clear();
        await InternalCommit(newEntityIds, newEvents);
    }

    protected abstract Task InternalCommit(IReadOnlyList<string> newEntityIds, IReadOnlyDictionary<string, IReadOnlyList<object>> newEvents);

    public async Task<TEntity?> Get<TEntity>(string entityId)
        where TEntity : AggregateRoot, IEntityCreator<TEntity>
    {
        var events = await GetStreamEvents(entityId);
        if (events.Count == 0)
        {
            return null;
        }

        var entity = TEntity.Create();
        entity.Load(events);
        return entity;
    }

    protected abstract Task<IReadOnlyList<object>> GetStreamEvents(string entityId);
}