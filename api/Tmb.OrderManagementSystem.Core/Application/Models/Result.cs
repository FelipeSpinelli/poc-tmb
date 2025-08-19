namespace Tmb.OrderManagementSystem.Core.Application.Models;

public class Result
{
    public bool IsValid => !Errors.Any();
    public IDictionary<string, IEnumerable<string>> Errors { get; } = new Dictionary<string, IEnumerable<string>>();
}
public class Result<T> : Result
{
    public T? Data { get; init; }

    public Result()
    {
    }

    public Result(T data)
    {
        Data = data;
    }
}
