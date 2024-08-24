using EventSourcingExercise.Transactions.Domains.Enums;

namespace EventSourcingExercise.Transactions.Domains;

public class Capture
{
    public required string CaptureId { get; init; }

    public EnumCaptureStatus Status { get; init; }
}