using Common.Errors;

namespace Domain.Common.Errors;

public class CardProviderError : Error
{
    public CardProviderError(string errorMessage) : base(ErrorCode.CardProviderError, errorMessage)
    { }
}