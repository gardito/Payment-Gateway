using Common.Errors;

namespace Domain.Common.Errors;

public class CardNumberError : Error
{
    public CardNumberError(string errorMessage) : base(ErrorCode.InvalidCardNumber, errorMessage)
    {
    }
}