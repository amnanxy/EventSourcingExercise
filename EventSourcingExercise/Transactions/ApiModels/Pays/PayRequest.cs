﻿using EventSourcingExercise.Transactions.Applications.Pays;

namespace EventSourcingExercise.Transactions.ApiModels.Pays;

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