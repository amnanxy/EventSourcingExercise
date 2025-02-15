namespace EventSourcingExercise.Infrastructures.EventSourcing.BackgroundServices.EventDeliveries;

[Alias("EventSourcingExercise.Infrastructures.IMissingEventDetectionGrain")]
public interface IMissingEventDetectionGrain : IGrainWithStringKey
{
    [Alias("Initial")]
    Task Initial();
}