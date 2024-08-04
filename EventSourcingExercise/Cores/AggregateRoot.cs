namespace EventSourcingExercise.Cores;

public abstract class AggregateRoot
{
    public string Id { get; protected set; } = default!;

    private readonly List<object> _events = [];

    public IReadOnlyList<object> GetEvents()
    {
        return _events.ToArray();
    }

    public void ClearEvents()
    {
        _events.Clear();
    }

    protected void Apply(object evt)
    {
        When(evt);
        _events.Add(evt);
    }

    protected abstract void When(object @event);

    protected static void RaiseEventOutOfRange(object evt)
    {
        throw new ArgumentOutOfRangeException(nameof(evt), $"event type: {evt}");
    }
}