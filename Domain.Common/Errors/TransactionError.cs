using Common.Errors;

namespace Domain.Common.Errors;

public class TransactionError : Error
{
    public TransactionError(string errorMessage) : base(ErrorCode.InvalidTransaction, errorMessage)
    {
    }
}