namespace EventSourcingExercise.Infrastructures;

public class EventData
{
    public long StreamId { get; init; }

    public long Version { get; init; }

    public required string EventText { get; init; }

    public required string EventName { get; init; }

    public DateTimeOffset CreatedAt { get; init; }
}