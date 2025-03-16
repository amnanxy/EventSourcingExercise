namespace EventSourcingExercise.Infrastructures.Projectors;

public class EventData<TEvent>
{
    public required TEvent Event { get; init; }

    public long StreamId { get; init; }

    public long EventId { get; init; }

    public required string TenantCode { get; init; }

    public int Version { get; init; }

    public DateTime CreatedAt { get; init; }
}