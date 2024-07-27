namespace EventSourcingExercise.Cores;

public abstract class AggregateRoot<T>
{
    public T Id { get; protected set; } = default!;

    protected abstract void When(object @event);
}