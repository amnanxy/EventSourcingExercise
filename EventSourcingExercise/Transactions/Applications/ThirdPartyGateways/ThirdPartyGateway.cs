using EventSourcingExercise.Utilities.Results;

namespace EventSourcingExercise.Transactions.Applications.ThirdPartyGateways;

public class ThirdPartyGateway : IThirdPartyGateway
{
    public Task<Result> Pay(ThirdPartyPaymentRequest thirdPartyPaymentRequest)
    {
        // always success
        return Task.FromResult(Result.Success());
    }
}