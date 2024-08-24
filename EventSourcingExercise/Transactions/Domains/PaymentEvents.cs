using EventSourcingExercise.Cores;
using EventSourcingExercise.Transactions.Domains.Enums;

namespace EventSourcingExercise.Transactions.Domains;

public static class PaymentEvents
{
    public record NewPaymentStarted(string Id, decimal Amount)
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

    public record CaptureAccepted
    {
        public required string CaptureId { get; set; }

        public EnumCaptureStatus Status => EnumCaptureStatus.Accepted;
    }
}