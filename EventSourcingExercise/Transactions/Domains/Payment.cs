namespace EventSourcingExercise.Transactions.Domains;

public class Payment
{
    public long Id { get; private init; }

    public decimal Amount { get; private init; }

    public EnumPaymentStatus Status { get; private set; }

    public static Payment StartNewPayment(long id, decimal amount)
    {
        return new Payment
        {
            Id = id,
            Amount = amount,
            Status = EnumPaymentStatus.Pending,
        };
    }

    public void PaySuccessful()
    {
        Status = EnumPaymentStatus.PaymentSuccess;
    }

    public void PayFailed()
    {
        Status = EnumPaymentStatus.PaymentFailure;
    }
}