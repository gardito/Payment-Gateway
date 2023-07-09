using Common.Errors;

namespace Common;

public class Result<T>
{
    public T Data;
    public bool IsSuccessful => string.IsNullOrWhiteSpace(Error?.ErrorMessage);
    public Error Error { get; }

    protected Result(T data)
    {
        Data = data;
    }

    protected Result(Error error)
    {
        Error = error;
    }
}

public class SuccessfulResult<T> : Result<T>
{
    public SuccessfulResult(T data) : base(data) { }
}

public class ErrorResult<T> : Result<T>
{
    public ErrorResult(Error error) : base(error) { }
}
