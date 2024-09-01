using EventSourcingExercise.Cores;
using EventSourcingExercise.Modules.Transactions.Applications.ThirdPartyGateways;
using EventSourcingExercise.Modules.Transactions.Domains;
using EventSourcingExercise.Utilities.IdGenerators;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Transactions.Applications.UseCases.Pays;

public class PayHandler : IRequestHandler<PayCommand, Result<PayResult?>>
{
    private readonly ITextIdGenerator _idGenerator;
    private readonly AggregateStoreBase _aggregateStore;
    private readonly IThirdPartyGateway _thirdPartyGateway;

    public PayHandler(
        ITextIdGenerator idGenerator,
        AggregateStoreBase aggregateStore,
        IThirdPartyGateway thirdPartyGateway)
    {
        _idGenerator = idGenerator;
        _aggregateStore = aggregateStore;
        _thirdPartyGateway = thirdPartyGateway;
    }

    public async Task<Result<PayResult?>> Handle(PayCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.StartNewPayment(_idGenerator.CreateId("PA", 12), request.Amount);

        _aggregateStore.Add(payment);

        await _aggregateStore.Commit();

        var result = await ProcessPayment(payment);

        _aggregateStore.Update(payment);

        await _aggregateStore.Commit();

        if (result.IsSuccess)
        {
            return Result<PayResult?>.Success(new PayResult
            {
                TransactionId = payment.Id,
            });
        }
        
        return Result<PayResult?>.Fail(result.Code);
    }

    private async Task<Result> ProcessPayment(Payment payment)
    {
        var result = await PayThroughThirdParty(payment);

        if (result.IsSuccess)
        {
            payment.PaySuccessful();
            return Result.Success();
        }

        payment.PayFailed();
        return Result.Fail(result.Code);
    }

    private async Task<Result> PayThroughThirdParty(Payment payment)
    {
        return await _thirdPartyGateway.Pay(new ThirdPartyPaymentRequest
        {
            Amount = payment.Amount,
        });
    }
}