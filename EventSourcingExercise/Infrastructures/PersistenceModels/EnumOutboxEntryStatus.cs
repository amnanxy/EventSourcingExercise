namespace EventSourcingExercise.Infrastructures.PersistenceModels;

public enum EnumOutboxEntryStatus
{
    Waiting = 1,
    Delivered,
}