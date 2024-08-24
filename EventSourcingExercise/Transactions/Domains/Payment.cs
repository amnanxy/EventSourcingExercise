using EventSourcingExercise.Cores;
using EventSourcingExercise.Transactions.Domains.Enums;
using static EventSourcingExercise.Transactions.Domains.PaymentEvents;

namespace EventSourcingExercise.Transactions.Domains;

public class Payment : AggregateRoot, IEntityCreator<Payment>
{
    private EnumPaymentStatus _status;

    public List<Capture> Captures { get; } = [];

    public decimal Amount { get; private set; }

    public EnumPaymentStatus Status
    {
        get
        {
            if (Captures.Count != 0)
            {
                return EnumPaymentStatus.Capturing;
            }

            return _status;
        }
    }

    private Payment()
    {
    }

    private Payment(string id, decimal amount)
    {
        Apply(new NewPaymentStarted(id, amount));
    }

    public static Payment Create()
    {
        return new Payment();
    }

    public static Payment StartNewPayment(string id, decimal amount)
    {
        return new Payment(id, amount);
    }

    public void PaySuccessful()
    {
        Apply(new PaymentSucceeded());
    }

    public void PayFailed()
    {
        Apply(new PaymentFailed());
    }

    public void AcceptCapture(string captureId)
    {
        Apply(new CaptureAccepted
        {
            CaptureId = captureId,
        });
    }

    protected override void When(object @event)
    {
        switch (@event)
        {
            case NewPaymentStarted evt:
                Id = evt.Id;
                Amount = evt.Amount;
                _status = evt.Status;
                break;
            case PaymentSucceeded evt:
                _status = evt.Status;
                break;
            case PaymentFailed evt:
                _status = evt.Status;
                break;
            case CaptureAccepted evt:
                Captures.Add(new Capture
                {
                    CaptureId = evt.CaptureId,
                    Status = EnumCaptureStatus.Accepted,
                });
                break;
            default:
                RaiseEventOutOfRange(@event);
                break;
        }
    }
}