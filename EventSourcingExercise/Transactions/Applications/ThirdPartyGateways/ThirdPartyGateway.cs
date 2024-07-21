using EventSourcingExercise.Utilities.Results;

namespace EventSourcingExercise.Transactions.Applications.ThirdPartyGateways;

public class ThirdPartyGateway : IThirdPartyGateway
{
    public async Task<Result> Pay(ThirdPartyPaymentRequest thirdPartyPaymentRequest)
    {
        // always success
        return Result.Success();
    }
}