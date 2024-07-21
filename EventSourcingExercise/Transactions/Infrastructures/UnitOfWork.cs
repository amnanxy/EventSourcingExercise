using EventSourcingExercise.Transactions.Applications;

namespace EventSourcingExercise.Transactions.Infrastructures;

public class UnitOfWork : IUnitOfWork
{
    public Task Commit()
    {
        return Task.CompletedTask;
    }
}