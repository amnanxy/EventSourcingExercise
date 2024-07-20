namespace EventSourcingExercise.Utilities.Results;

public class Result<T> : Result
{
    public T? Data { get; init; }
    
    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            Data = data,
            Code = "Success",
        };
    }

    public static Result<T?> Fail(string code, T? data = default)
    {
        return new Result<T?>
        {
            Data = data,
            Code = code,
        };
    }


}