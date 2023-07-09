using Common;
using Domain.Common.Errors;

namespace Domain.ValueObjects;

public enum CardProviderEnum
{
    Visa,
    Mastercard,
    AmericanExpress,
    Unknown = 0 // default value in case of error
}

public class CardProvider
{
    public static Result<CardProviderEnum> GetProvider(CardNumber cardNumber)
    {
        var cardDigits = cardNumber.Get;
        if (cardDigits.StartsWith("2") || cardDigits.StartsWith("5"))
            return new SuccessfulResult<CardProviderEnum>(CardProviderEnum.Mastercard);
        if (cardDigits.StartsWith("4"))
            return new SuccessfulResult<CardProviderEnum>(CardProviderEnum.Visa);
        if (cardDigits.StartsWith("34") || cardDigits.StartsWith("37"))
            return new SuccessfulResult<CardProviderEnum>(CardProviderEnum.AmericanExpress);
        // this is an edge case
        return new ErrorResult<CardProviderEnum>(new CardProviderError("Card can only be Visa, Mastercard or American Express"));
    }
    
    public static Result<CardProviderEnum> ValidateProvider(CardNumber cardNumber, Cvv cvv)
    {
        var cardProvider = GetProvider(cardNumber);
        
        if(!cardProvider.IsSuccessful)
            return new ErrorResult<CardProviderEnum>(new CardProviderError("Invalid Card Provider"));

        // Visa or Mastercard
        if (Cvv.IsAmex(cvv.Get.ToString()) 
            && (cardProvider.Data == CardProviderEnum.Visa || cardProvider.Data == CardProviderEnum.Mastercard))
            return new ErrorResult<CardProviderEnum>(new CardProviderError("Invalid CVV for a Visa or Mastercard card"));
        
        // Amex
        if(!Cvv.IsAmex(cvv.Get.ToString())
           && cardProvider.Data == CardProviderEnum.AmericanExpress)
            return new ErrorResult<CardProviderEnum>(new CardProviderError("Invalid CVV for American Express card"));

        return new SuccessfulResult<CardProviderEnum>(cardProvider.Data);
    }
}