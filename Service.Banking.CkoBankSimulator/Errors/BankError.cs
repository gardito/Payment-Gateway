using Common.Errors;

namespace Service.Banking.CkoBankSimulator.Errors;

public class BankError : Error
{
    public BankError(string errorMessage) : base(ErrorCode.BankError, errorMessage)
    { }
}