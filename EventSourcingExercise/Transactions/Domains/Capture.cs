using EventSourcingExercise.Cores;
using EventSourcingExercise.Transactions.Domains.Enums;
using static EventSourcingExercise.Transactions.Domains.PaymentEvents;

namespace EventSourcingExercise.Transactions.Domains;

public class Capture : EntityBase
{
    public decimal Amount { get; private set; }

    public EnumCaptureStatus Status { get; private set; }

    public Capture(Action<object> applier) : base(applier)
    {
    }

    protected override void When(object evt)
    {
        switch (evt)
        {
            case CaptureAccepted ev:
                Id = ev.CaptureId;
                Status = EnumCaptureStatus.Accepted;
                Amount = ev.Amount;
                break;
        }
    }
}