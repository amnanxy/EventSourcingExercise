namespace EventSourcingExercise.Cores;

public abstract class EntityBase : IInternalEventHandler
{
    private readonly Action<object> _applier;

    protected EntityBase(Action<object> applier) => _applier = applier;

    public string Id { get; protected set; } = null!;
    
    protected void Apply(object evt)
    {
        When(evt);
        _applier(evt);
    }

    void IInternalEventHandler.Handle(object evt) => When(evt);

    protected abstract void When(object @event);
}