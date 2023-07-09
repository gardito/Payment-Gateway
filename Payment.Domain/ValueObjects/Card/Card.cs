using Common;

namespace Domain.ValueObjects;

public class Card
{
    public readonly FullName OwnerFullName;
    public readonly CardNumber CardNumber;
    public readonly CardProviderEnum Provider;
    public readonly Cvv Cvv;
    public readonly Month ExpiryMonth;
    public readonly Year ExpiryYear;

    private Card(FullName ownerFullName, CardNumber cardNumber, Cvv cvv, CardProviderEnum provider, ExpiryDate expiryDate)
    {
        OwnerFullName = ownerFullName;
        CardNumber = cardNumber;
        Cvv = cvv;
        Provider = provider;
        ExpiryMonth = expiryDate.Month;
        ExpiryYear = expiryDate.Year;
    }

    public static Result<Card> Create(string ownerFullName, string cardNumber, string cvv, string month, string year)
    {
        var fullNameResult = FullName.Create(ownerFullName);
        var cardNumberResult = CardNumber.Create(cardNumber);
        var cvvResult = Cvv.Create(cvv);
        var monthResult = Month.Create(month);
        var yearResult = Year.Create(year);
        var expiryDateResult = ExpiryDate.Create(monthResult.Data, yearResult.Data);

        if (!fullNameResult.IsSuccessful)
            return new ErrorResult<Card>(fullNameResult.Error);
        if (!cardNumberResult.IsSuccessful)
            return new ErrorResult<Card>(cardNumberResult.Error);
        if (!cvvResult.IsSuccessful)
            return new ErrorResult<Card>(cvvResult.Error);
        
        var cardProviderValidation = CardProvider.ValidateProvider(cardNumberResult.Data, cvvResult.Data);
        
        if (!cardProviderValidation.IsSuccessful)
            return new ErrorResult<Card>(cardProviderValidation.Error);
        if (!monthResult.IsSuccessful)
            return new ErrorResult<Card>(monthResult.Error);
        if (!yearResult.IsSuccessful)
            return new ErrorResult<Card>(yearResult.Error);
        if (!expiryDateResult.IsSuccessful)
            return new ErrorResult<Card>(expiryDateResult.Error);

        return new SuccessfulResult<Card>(
            new Card(
                fullNameResult.Data,
                cardNumberResult.Data,
                cvvResult.Data,
                cardProviderValidation.Data,
                expiryDateResult.Data));
    }

    public string GetMaskedNumber()
    {
        var cardNumber = CardNumber.Get;
        var maskedNumbers = string.Concat(cardNumber.Substring(0, cardNumber.Length - 5).Select(_ => '*'));
        var lastDigits = cardNumber.Substring(cardNumber.Length - 4);
        return maskedNumbers + lastDigits;
    }
}