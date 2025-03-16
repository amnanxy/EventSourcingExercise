namespace EventSourcingExercise.Infrastructures.EventSourcing.BackgroundServices.EventDeliveries;

[GenerateSerializer]
[Alias("EventSourcingExercise.Infrastructures.BackgroundServices.EventDeliveries.EventItem")]
public class EventItem
{
    [Id(0)] 
    public long StreamId { get; init; }

    [Id(1)] 
    public long EventId { get; init; }

    [Id(2)]
    public required string TenantCode { get; init; }

    [Id(3)]
    public int Version { get; init; }

    [Id(4)]
    public required string EventText { get; init; }

    [Id(5)]
    public required string EventName { get; init; }

    [Id(6)]
    public DateTime CreatedAt { get; init; }
}