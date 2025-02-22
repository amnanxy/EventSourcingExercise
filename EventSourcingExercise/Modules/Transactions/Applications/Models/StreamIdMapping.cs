namespace EventSourcingExercise.Modules.Transactions.Applications.Models;

public class StreamIdMapping
{
    public long StreamId { get; init; }
    public required string AggregateCode { get; init; }
}