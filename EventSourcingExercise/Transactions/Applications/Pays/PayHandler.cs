using EventSourcingExercise.Transactions.Applications.ThirdPartyGateways;
using EventSourcingExercise.Transactions.Domains;
using EventSourcingExercise.Utilities.IdGenerators;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Transactions.Applications.Pays;

public class PayHandler : IRequestHandler<PayCommand, Result>
{
    private readonly IIdGenerator<long> _idGenerator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IThirdPartyGateway _thirdPartyGateway;

    public PayHandler(
        IIdGenerator<long> idGenerator,
        IUnitOfWork unitOfWork,
        IPaymentRepository paymentRepository,
        IThirdPartyGateway thirdPartyGateway)
    {
        _idGenerator = idGenerator;
        _unitOfWork = unitOfWork;
        _paymentRepository = paymentRepository;
        _thirdPartyGateway = thirdPartyGateway;
    }

    public async Task<Result> Handle(PayCommand request, CancellationToken cancellationToken)
    {
        var payment = Payment.StartNewPayment(_idGenerator.CreateId(), request.Amount);

        _paymentRepository.Add(payment);

        var result = await ProcessPayment(payment);

        _paymentRepository.Update(payment);

        await _unitOfWork.Commit();

        return result;
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