namespace Common.Errors;

public enum ErrorCode
{
    InvalidName,
    InvalidCardNumber,
    InvalidCvv,
    CardProviderError,
    InvalidDate,
    InvalidExpiryDate,
    InvalidTransaction,
    PaymentNotFound,
    BankError,
    DatabaseError,
    SystemError
}