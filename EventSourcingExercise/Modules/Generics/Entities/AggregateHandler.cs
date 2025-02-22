using EventSourcingExercise.Cores;
using EventSourcingExercise.Infrastructures.Payments;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Generics.Entities;

public class AggregateHandler<T> : IRequestHandler<AggregateQuery<T>, Result<T?>>
    where T : AggregateRoot
{
    private readonly AggregateStoreBase _aggregateStore;
    private readonly PaymentReadonlyDbContext _paymentReadonlyDbContext;

    public AggregateHandler(AggregateStoreBase aggregateStore, PaymentReadonlyDbContext paymentReadonlyDbContext)
    {
        _aggregateStore = aggregateStore;
        _paymentReadonlyDbContext = paymentReadonlyDbContext;
    }

    public async Task<Result<T?>> Handle(AggregateQuery<T> request, CancellationToken cancellationToken)
    {
        var idMapping = (await _paymentReadonlyDbContext.StreamIdMappings.FindAsync(request.AggregateId))!;
        var aggregate = await _aggregateStore.Get<T>(idMapping.StreamId);
        return aggregate == null
            ? Result<T?>.Fail("NotFound")
            : Result<T?>.Success(aggregate);
    }
}