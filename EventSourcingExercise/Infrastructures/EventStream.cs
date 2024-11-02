namespace EventSourcingExercise.Infrastructures;

public class EventStream
{
    public long StreamId { get; internal init; }

    public string AggregateRootTypeName { get; internal init; } = null!;

    public int Version { get; internal set; }
}