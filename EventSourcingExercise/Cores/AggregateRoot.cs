namespace EventSourcingExercise.Cores;

public abstract class AggregateRoot : IInternalEventHandler
{
    public string Id { get; protected set; } = default!;

    private readonly List<object> _events = [];

    public IReadOnlyList<object> GetEvents() => _events.ToArray();

    internal int EventCount => _events.Count;

    internal void ClearEvents() => _events.Clear();

    protected void Apply(object evt)
    {
        When(evt);
        _events.Add(evt);
    }

    internal void Load(IEnumerable<object> events)
    {
        foreach (var evt in events)
        {
            When(evt);
        }
    }

    protected abstract void When(object evt);

    protected void ApplyToEntity(IInternalEventHandler entity, object evt)
    {
        entity.Handle(evt);
    }

    void IInternalEventHandler.Handle(object evt) => When(evt);
}