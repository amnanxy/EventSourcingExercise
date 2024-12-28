namespace EventSourcingExercise.Infrastructures.PersistenceModels;

public class EventStream
{
    public long Id { get; internal init; }

    public string AggregateRootTypeName { get; internal init; } = null!;

    public sbyte Version { get; internal set; }
}