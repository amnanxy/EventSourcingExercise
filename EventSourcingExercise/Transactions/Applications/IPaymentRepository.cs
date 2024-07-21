using EventSourcingExercise.Transactions.Domains;

namespace EventSourcingExercise.Transactions.Applications;

public interface IPaymentRepository
{
    void Add(Payment payment);
    void Update(Payment payment);
}