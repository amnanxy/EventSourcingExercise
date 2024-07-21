namespace EventSourcingExercise.Utilities.IdGenerators;

public interface IIdGenerator<out T>
{
    T CreateId();
}