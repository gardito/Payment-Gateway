using Common.Errors;

namespace Domain.Common.Errors;

public class ExpiryDateError : Error
{
    public ExpiryDateError(string errorMessage)  : base(ErrorCode.InvalidExpiryDate, errorMessage)
    { }
}