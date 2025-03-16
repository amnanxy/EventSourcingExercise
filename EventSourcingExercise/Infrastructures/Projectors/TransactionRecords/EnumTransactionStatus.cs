namespace EventSourcingExercise.Infrastructures.Projectors.TransactionRecords;

public static class EnumTransactionStatus
{
    public static readonly string PaymentPending = nameof(PaymentPending);
    public static readonly string PaymentSuccess = nameof(PaymentSuccess);
}