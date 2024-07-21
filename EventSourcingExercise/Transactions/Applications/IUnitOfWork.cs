namespace EventSourcingExercise.Transactions.Applications;

public interface IUnitOfWork
{
    Task Commit();
}