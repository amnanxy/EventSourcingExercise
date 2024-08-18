using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingExercise.Controllers;

public class CommonControllerBase : ControllerBase
{
    protected readonly IMediator Mediator;

    public CommonControllerBase(IMediator mediator)
    {
        Mediator = mediator;
    }
}