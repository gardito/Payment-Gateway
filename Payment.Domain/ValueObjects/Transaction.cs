using Common;

namespace Domain.ValueObjects;

public enum Currency
{
    GBP,
    EUR,
    USD
}

public class Transaction
{
    // using decimal as per https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/floating-point-numeric-types
    public decimal Amount;
    public Currency Currency;

    public Transaction(decimal amount, Currency currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Result<Transaction> Create(decimal amount, Currency currency)
    {
        return new SuccessfulResult<Transaction>(new Transaction(amount, currency));
    }
}