using Common.Errors;

namespace Domain.Common.Errors;

public class CvvError : Error
{
    public CvvError(string errorMessage) : base(ErrorCode.InvalidCvv, errorMessage)
    {
    }
}