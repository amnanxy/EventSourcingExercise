namespace EventSourcingExercise.Infrastructures.Projectors.TransactionRecords;

public class TransactionRecord : ProjectorEntryBase
{
    public required string PaymentId { get; init; }

    public decimal Amount { get; init; }

    public required string Status { get; set; }
}