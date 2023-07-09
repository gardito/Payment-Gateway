using Common.Errors;

namespace Database.Common.Errors;

public class SystemError : Error
{
    public SystemError(string errorMessage) : base(ErrorCode.SystemError, errorMessage)
    { }
}