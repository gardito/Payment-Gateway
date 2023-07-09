namespace Common.Errors;

public abstract class Error
{
    public ErrorCode ErrorCode { get; }
    public string ErrorMessage { get; }

    protected Error(ErrorCode errorCode, string errorMessage)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}