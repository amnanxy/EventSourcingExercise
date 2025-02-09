namespace EventSourcingExercise.Infrastructures.PersistenceModels;

public class EventEntry
{
    public long Id { get; init; }

    public long StreamId { get; init; }

    public int Version { get; init; }

    public required string EventText { get; init; }

    public required string EventName { get; init; }

    public DateTime CreatedAt { get; init; }
}