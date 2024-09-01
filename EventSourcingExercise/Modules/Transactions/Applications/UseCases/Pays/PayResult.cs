namespace EventSourcingExercise.Modules.Transactions.Applications.UseCases.Pays;

public class PayResult
{
    public required string TransactionId { get; init; }
}