namespace EventSourcingExercise.Utilities.Results;

public class Result
{
    private static readonly Result SuccessResult = new()
    {
        Code = "Success",
    };

    protected Result()
    {
    }

    public required string Code { get; init; }

    public bool IsSuccess => Code == "Success";

    public static Result Success()
    {
        return SuccessResult;
    }

    public static Result Fail(string code)
    {
        return new Result
        {
            Code = code,
        };
    }
}