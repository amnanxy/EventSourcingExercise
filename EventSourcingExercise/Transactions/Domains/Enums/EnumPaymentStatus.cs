namespace EventSourcingExercise.Transactions.Domains.Enums;

public enum EnumPaymentStatus
{
    PaymentPending = 1,
    PaymentSuccess,
    PaymentFailure,
    Capturing,
}