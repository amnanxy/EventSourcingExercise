using EventSourcingExercise.Cores;
using static EventSourcingExercise.Transactions.Domains.PaymentEvents;

namespace EventSourcingExercise.Transactions.Domains;

public class Payment : AggregateRoot<long>
{
    public decimal Amount { get; private set; }

    public EnumPaymentStatus Status { get; private set; }

    private Payment(long id, decimal amount)
    {
        Apply(new NewPaymentStarted(id, amount));
    }

    public static Payment StartNewPayment(long id, decimal amount)
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

    private void Apply(object evt)
    {
        When(evt);
    }

    protected override void When(object @event)
    {
        switch (@event)
        {
            case NewPaymentStarted evt:
                Id = evt.Id;
                Amount = evt.Amount;
                Status = evt.Status;
                break;
            case PaymentSucceeded evt:
                Status = evt.Status;
                break;
            case PaymentFailed evt:
                Status = evt.Status;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(@event), $"event type: {@event}");
        }
    }
}