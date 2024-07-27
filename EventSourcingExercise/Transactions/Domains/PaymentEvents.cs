namespace EventSourcingExercise.Transactions.Domains;

public static class PaymentEvents
{
    public record NewPaymentStarted(long Id, decimal Amount)
    {
        public EnumPaymentStatus Status => EnumPaymentStatus.Pending;
    }

    public record PaymentFailed
    {
        public EnumPaymentStatus Status => EnumPaymentStatus.PaymentFailure;
    }

    public record PaymentSucceeded
    {
        public EnumPaymentStatus Status => EnumPaymentStatus.PaymentSuccess;
    }
}