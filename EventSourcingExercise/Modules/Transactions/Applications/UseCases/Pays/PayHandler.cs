using EventSourcingExercise.Cores;
using EventSourcingExercise.Infrastructures.Payments;
using EventSourcingExercise.Modules.Transactions.Applications.Models;
using EventSourcingExercise.Modules.Transactions.Applications.ThirdPartyGateways;
using EventSourcingExercise.Modules.Transactions.Domains;
using EventSourcingExercise.Utilities.IdGenerators;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Transactions.Applications.UseCases.Pays;

public class PayHandler : IRequestHandler<PayCommand, Result<PayResult?>>
{
    private readonly ITextIdGenerator _idGenerator;
    private readonly PaymentDbContext _paymentDbContext;
    private readonly AggregateStoreBase _aggregateStore;
    private readonly IThirdPartyGateway _thirdPartyGateway;

    public PayHandler(
        ITextIdGenerator idGenerator,
        PaymentDbContext paymentDbContext,
        AggregateStoreBase aggregateStore,
        IThirdPartyGateway thirdPartyGateway)
    {
        _idGenerator = idGenerator;
        _paymentDbContext = paymentDbContext;
        _aggregateStore = aggregateStore;
        _thirdPartyGateway = thirdPartyGateway;
    }

    public async Task<Result<PayResult?>> Handle(PayCommand request, CancellationToken cancellationToken)
    {
        var payment = await StartNewPayment(request, cancellationToken);

        var result = await ProcessPayment(payment);

        if (result.IsSuccess)
        {
            return Result<PayResult?>.Success(new PayResult
            {
                TransactionId = payment.Id,
            });
        }

        return Result<PayResult?>.Fail(result.Code);
    }

    private async Task<Payment> StartNewPayment(PayCommand request, CancellationToken cancellationToken)
    {
        var paymentCode = _idGenerator.CreateId("PA", 12);

        var payment = Payment.StartNewPayment(paymentCode, request.Amount);

        var streamId = _aggregateStore.Add(payment);

        var idMapping = new StreamIdMapping { StreamId = streamId, AggregateCode = paymentCode };

        await _paymentDbContext.StreamIdMappings.AddAsync(idMapping, cancellationToken);

        await _paymentDbContext.SaveChangesAsync(cancellationToken);

        await _aggregateStore.Commit();

        return payment;
    }

    private async Task<Result> ProcessPayment(Payment payment)
    {
        var result = await PayThroughThirdParty(payment);

        if (result.IsSuccess)
        {
            payment.PaySuccessful();
        }
        else
        {
            payment.PayFailed();
        }

        _aggregateStore.Update(payment);

        await _aggregateStore.Commit();

        return result;
    }

    private async Task<Result> PayThroughThirdParty(Payment payment)
    {
        return await _thirdPartyGateway.Pay(new ThirdPartyPaymentRequest
        {
            Amount = payment.Amount,
        });
    }
}