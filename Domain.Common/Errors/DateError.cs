using Common.Errors;

namespace Domain.Common.Errors;

public class DateError : Error
{
    public DateError(string errorMessage) : base(ErrorCode.InvalidDate, errorMessage)
    {
    }
}
