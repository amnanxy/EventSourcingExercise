using EventSourcingExercise.Utilities.Results;

namespace EventSourcingExercise.Modules.Transactions.Applications.ThirdPartyGateways;

public interface IThirdPartyGateway
{
    Task<Result> Pay(ThirdPartyPaymentRequest thirdPartyPaymentRequest);
}