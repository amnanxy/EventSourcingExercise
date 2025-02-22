namespace EventSourcingExercise.Infrastructures.EventSourcing.Models;

public class EventStream
{
    public long Id { get; init; }

    public required string AggregateRootTypeName { get; init; }

    public int Version { get; internal set; }

    public required string TenantCode { get; init; }
}