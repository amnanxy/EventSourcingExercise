using EventSourcingExercise.Utilities.Results;

namespace EventSourcingExercise.Transactions.Applications.ThirdPartyGateways;

public interface IThirdPartyGateway
{
    Task<Result> Pay(ThirdPartyPaymentRequest thirdPartyPaymentRequest);
}