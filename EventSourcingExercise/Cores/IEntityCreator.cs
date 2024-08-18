namespace EventSourcingExercise.Cores;

public interface IEntityCreator<out TEntity>
{
    internal static abstract TEntity Create();
}