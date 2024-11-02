using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Generics.Entities;

public class AggregateHandler<T> : IRequestHandler<AggregateQuery<T>, Result<T?>>
    where T : AggregateRoot
{
    private readonly AggregateStoreBase _aggregateStore;

    public AggregateHandler(AggregateStoreBase aggregateStore)
    {
        _aggregateStore = aggregateStore;
    }

    public async Task<Result<T?>> Handle(AggregateQuery<T> request, CancellationToken cancellationToken)
    {
        var aggregate = await _aggregateStore.Get<T>(request.AggregateId);
        return aggregate == null
            ? Result<T?>.Fail("NotFound")
            : Result<T?>.Success(aggregate);
    }
}