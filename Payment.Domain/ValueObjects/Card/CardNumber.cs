using System.Text.RegularExpressions;
using Common;
using Domain.Common.Errors;

namespace Domain.ValueObjects;

public class CardNumber
{
    public string Get { get; }
    
    private const int VisaOrMastercardDigits = 16;
    private const int AmericanExpressDigits = 15;

    private CardNumber(string cardNumber)
    {
        Get = cardNumber;
    }

    public static Result<CardNumber> Create(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return new ErrorResult<CardNumber>(new CardNumberError("Card number is empty"));
        
        if(!Regex.IsMatch(cardNumber, @"^[0-9]+$"))
            return new ErrorResult<CardNumber>(new CardNumberError("Card number can contain digits only"));
        
        if(!IsVisaMastercardOrAmex(cardNumber))
            return new ErrorResult<CardNumber>(new CardNumberError("Card number can contain either 15 or 16 digits"));

        return new SuccessfulResult<CardNumber>(new CardNumber(cardNumber));
    }

    private static bool IsVisaMastercardOrAmex(string cardNumber) => 
        cardNumber.Length is VisaOrMastercardDigits or AmericanExpressDigits;
    
}