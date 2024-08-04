namespace EventSourcingExercise.Infrastructures;

public class EventStream
{
    public long StreamId { get; internal init; }

    public int Version { get; internal set; }
}