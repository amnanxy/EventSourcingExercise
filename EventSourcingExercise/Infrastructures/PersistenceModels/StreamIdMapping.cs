namespace EventSourcingExercise.Infrastructures.PersistenceModels;

public class StreamIdMapping
{
    public long StreamId { get; init; }
    public required string AggregateId { get; init; }
}