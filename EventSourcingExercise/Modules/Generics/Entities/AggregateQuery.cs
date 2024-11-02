using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Generics.Entities;

public class AggregateQuery<T> : IRequest<Result<T?>>
    where T : AggregateRoot
{
    public required string AggregateId { get; init; }
}