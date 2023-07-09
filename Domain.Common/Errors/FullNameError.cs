using Common.Errors;

namespace Domain.Common.Errors;

public class FullNameError : Error
{
    public FullNameError(string errorMessage) : base(ErrorCode.InvalidName, errorMessage)
    {
    }
}