namespace EventSourcingExercise.Cores;

public interface IInternalEventHandler
{
    void Handle(object evt);
}