namespace EventSourcingExercise.Infrastructures;

[GenerateSerializer]
[Alias("EventSourcingExercise.Infrastructures.EventData")]
public class EventData
{
    [Id(0)]
    public long StreamId { get; init; }

    [Id(1)]
    public long Version { get; init; }

    [Id(2)]
    public required string EventText { get; init; }

    [Id(3)]
    public required string EventName { get; init; }

    [Id(4)]
    public DateTimeOffset CreatedAt { get; init; }
}