using EventSourcingExercise.Cores;
using EventSourcingExercise.Infrastructures.Payments;
using EventSourcingExercise.Modules.Transactions.Domains;
using EventSourcingExercise.Utilities.IdGenerators;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Transactions.Applications.UseCases.Captures;

public class CaptureHandler : IRequestHandler<CaptureCommand, Result<CaptureResult?>>
{
    private readonly ITextIdGenerator _idGenerator;
    private readonly AggregateStoreBase _aggregateStore;
    private readonly PaymentReadonlyDbContext _paymentReadonlyDbContext;

    public CaptureHandler(ITextIdGenerator idGenerator, AggregateStoreBase aggregateStore, PaymentReadonlyDbContext paymentReadonlyDbContext)
    {
        _idGenerator = idGenerator;
        _aggregateStore = aggregateStore;
        _paymentReadonlyDbContext = paymentReadonlyDbContext;
    }

    public async Task<Result<CaptureResult?>> Handle(CaptureCommand request, CancellationToken cancellationToken)
    {
        var idMapping = (await _paymentReadonlyDbContext.StreamIdMappings.FindAsync(request.TransactionId))!;

        var payment = await _aggregateStore.Get<Payment>(idMapping.StreamId);

        if (payment == null)
        {
            return Result<CaptureResult?>.Fail("PaymentNotFound");
        }

        var captureId = _idGenerator.CreateId("CP", 12);

        payment.AcceptCapture(captureId);

        _aggregateStore.Update(payment);

        await _aggregateStore.Commit();

        return Result<CaptureResult?>.Success(new CaptureResult
        {
            CaptureId = captureId,
        });
    }
}