namespace EventSourcingExercise.Infrastructures.BackgroundServices.EventDeliveries;

[Alias("EventSourcingExercise.Infrastructures.IMissingEventDetectionGrain")]
public interface IMissingEventDetectionGrain : IGrainWithStringKey
{
    [Alias("Initial")]
    Task Initial();
}