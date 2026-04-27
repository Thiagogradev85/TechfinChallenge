namespace TechfinChallenge.Shared;

public class Result<T>
{
    public T? Data { get; init; }
    public string? Error { get; init; }
    public bool IsSuccess => Error is null;

    public static Result<T> Success(T data) => new() { Data = data };
    public static Result<T> Failure(string error) => new() { Error = error };
}
