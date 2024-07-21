namespace EventSourcingExercise.Transactions.Domains;

public enum EnumPaymentStatus
{
    Pending = 1,
    PaymentSuccess,
    PaymentFailure,
}