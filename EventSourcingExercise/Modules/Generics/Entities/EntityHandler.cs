using EventSourcingExercise.Cores;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Generics.Entities;

public class EntityHandler<T> : IRequestHandler<EntityQuery<T>, Result<T?>>
    where T : AggregateRoot, IEntityCreator<T>
{
    private readonly AggregationStoreBase _aggregationStore;

    public EntityHandler(AggregationStoreBase aggregationStore)
    {
        _aggregationStore = aggregationStore;
    }

    public async Task<Result<T?>> Handle(EntityQuery<T> request, CancellationToken cancellationToken)
    {
        var entity = await _aggregationStore.Get<T>(request.EntityId);
        return entity == null
            ? Result<T?>.Fail("NotFound")
            : Result<T?>.Success(entity);
    }
}