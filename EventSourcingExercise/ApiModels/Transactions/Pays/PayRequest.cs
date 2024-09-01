using EventSourcingExercise.Modules.Transactions.Applications.UseCases.Pays;

namespace EventSourcingExercise.ApiModels.Transactions.Pays;

public class PayRequest
{
    public decimal Amount { get; init; }

    public PayCommand ToCommand()
    {
        return new PayCommand
        {
            Amount = Amount,
        };
    }
}