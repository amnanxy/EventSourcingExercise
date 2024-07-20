namespace EventSourcingExercise.Utilities.Results;

public class Result
{
    protected Result()
    {
    }

    public required string Code { get; init; }

    public bool IsSuccess => Code == "Success";

    public static Result Success()
    {
        return new Result
        {
            Code = "Success",
        };
    }

    public static Result Fail(string code)
    {
        return new Result
        {
            Code = code,
        };
    }
}