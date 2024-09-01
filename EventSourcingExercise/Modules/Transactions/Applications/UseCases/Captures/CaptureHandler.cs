using EventSourcingExercise.Cores;
using EventSourcingExercise.Modules.Transactions.Domains;
using EventSourcingExercise.Utilities.IdGenerators;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Transactions.Applications.UseCases.Captures;

public class CaptureHandler : IRequestHandler<CaptureCommand, Result<CaptureResult?>>
{
    private readonly ITextIdGenerator _idGenerator;
    private readonly AggregationStoreBase _aggregationStore;

    public CaptureHandler(ITextIdGenerator idGenerator, AggregationStoreBase aggregationStore)
    {
        _idGenerator = idGenerator;
        _aggregationStore = aggregationStore;
    }

    public async Task<Result<CaptureResult?>> Handle(CaptureCommand request, CancellationToken cancellationToken)
    {
        var payment = await _aggregationStore.Get<Payment>(request.TransactionId);

        if (payment == null)
        {
            return Result<CaptureResult?>.Fail("PaymentNotFound");
        }

        var captureId = _idGenerator.CreateId("CP", 12);

        payment.AcceptCapture(captureId);

        _aggregationStore.Update(payment);

        await _aggregationStore.Commit();

        return Result<CaptureResult?>.Success(new CaptureResult
        {
            CaptureId = captureId,
        });
    }
}