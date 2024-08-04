namespace EventSourcingExercise.Utilities.IdGenerators;

public interface ITextIdGenerator
{
    string CreateId(string prefix, int randomLength);
}