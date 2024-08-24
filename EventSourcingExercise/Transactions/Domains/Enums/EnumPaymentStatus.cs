namespace EventSourcingExercise.Transactions.Domains.Enums;

public enum EnumPaymentStatus
{
    Pending = 1,
    PaymentSuccess,
    PaymentFailure,
    Capturing,
}