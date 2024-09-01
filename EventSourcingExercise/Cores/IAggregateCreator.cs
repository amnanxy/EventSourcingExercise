namespace EventSourcingExercise.Cores;

public interface IAggregateCreator<out TAggregate>
{
    internal static abstract TAggregate Create();
}