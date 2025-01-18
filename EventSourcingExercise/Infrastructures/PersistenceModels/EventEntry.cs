namespace EventSourcingExercise.Infrastructures.PersistenceModels;

[GenerateSerializer]
[Alias("EventSourcingExercise.Infrastructures.EventData")]
public class EventEntry
{
    [Id(1)] 
    public long Id { get; init; }
    
    [Id(2)]
    public long StreamId { get; init; }

    [Id(3)]
    public int Version { get; init; }

    [Id(4)]
    public required string EventText { get; init; }

    [Id(5)]
    public required string EventName { get; init; }

    [Id(6)]
    public DateTimeOffset CreatedAt { get; init; }
}