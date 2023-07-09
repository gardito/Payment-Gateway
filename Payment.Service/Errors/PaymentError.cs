using Common.Errors;

namespace Payment.Service;

public class PaymentError : Error
{
    public PaymentError(string errorMessage) : base(ErrorCode.PaymentNotFound, errorMessage)
    { }
}