using EventSourcingExercise.Cores;
using EventSourcingExercise.Transactions.Domains.Enums;
using static EventSourcingExercise.Transactions.Domains.PaymentEvents;

namespace EventSourcingExercise.Transactions.Domains;

public class Payment : AggregateRoot, IEntityCreator<Payment>
{
    private EnumPaymentStatus _status;
    private readonly List<Capture> _captures = [];

    public IReadOnlyList<Capture> Captures => _captures;

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

    private Payment(string paymentId, decimal amount)
    {
        Apply(new NewPaymentStarted(paymentId, amount));
    }

    static Payment IEntityCreator<Payment>.Create()
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
        Apply(new CaptureAccepted(captureId, Amount));
    }

    protected override void When(object evt)
    {
        switch (evt)
        {
            case NewPaymentStarted e:
                Id = e.PaymentId;
                Amount = e.Amount;
                _status = EnumPaymentStatus.PaymentPending;
                break;
            case PaymentSucceeded:
                _status = EnumPaymentStatus.PaymentSuccess;
                break;
            case PaymentFailed:
                _status = EnumPaymentStatus.PaymentFailure;
                break;
            case CaptureAccepted e:
                var capture = new Capture(Apply);
                ApplyToEntity(capture, e);
                _captures.Add(capture);
                break;
        }
    }
}