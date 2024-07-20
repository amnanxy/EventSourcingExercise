using EventSourcingExercise.Transactions.Domains.Pays;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Transactions.Applications.Pays;

public class PayHandler : IRequestHandler<PayCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;

    public PayHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<Result> Handle(PayCommand request, CancellationToken cancellationToken)
    {
        return Result.Success();
    }
}