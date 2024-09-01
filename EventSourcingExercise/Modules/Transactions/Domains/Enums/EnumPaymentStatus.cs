namespace EventSourcingExercise.Modules.Transactions.Domains.Enums;

public enum EnumPaymentStatus
{
    PaymentPending = 1,
    PaymentSuccess,
    PaymentFailure,
    Capturing,
}