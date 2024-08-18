using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Generics.Entities;

public class EntityQuery<T> : IRequest<Result<T?>>
    where T : AggregateRoot, IEntityCreator<T>
{
    public required string EntityId { get; init; }
}