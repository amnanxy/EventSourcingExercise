namespace EventSourcingExercise.Modules.Transactions.Domains;

public static class PaymentEvents
{
    public record NewPaymentStarted(string PaymentId, decimal Amount);

    public record PaymentFailed;

    public record PaymentSucceeded;

    public record CaptureAccepted(string CaptureId, decimal Amount);
}