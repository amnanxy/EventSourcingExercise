namespace EventSourcingExercise.Utilities.Results;

public class Result<T> : Result
{
    private Result()
    {
    }

    public T? Data { get; init; }
    
    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            Code = "Success",
            Data = data,
        };
    }

    public static Result<T> Fail(string code, T? data = default)
    {
        return new Result<T>
        {
            Code = code,
            Data = data,
        };
    }


}