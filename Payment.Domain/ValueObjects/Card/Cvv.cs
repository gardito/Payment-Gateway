using System.Text.RegularExpressions;
using Common;
using Domain.Common.Errors;

namespace Domain.ValueObjects;

public class Cvv
{
    public int Get { get; }

    private const int VisaOrMastercardCvv = 3;
    private const int AmericanExpressCvv = 4;

    private Cvv(int cvv)
    {
        Get = cvv;
    }

    public static Result<Cvv> Create(string cvv)
    {
        if (string.IsNullOrWhiteSpace(cvv))
            return new ErrorResult<Cvv>(new CvvError("Cvv number is empty"));
        if (!Regex.IsMatch(cvv, @"^[0-9]+$"))
            return new ErrorResult<Cvv>(new CvvError("Cvv number can contain digits only"));
        if (!IsVisaMastercardOrAmex(cvv))
            return new ErrorResult<Cvv>(new CvvError("Cvv number can contain 3 or 4 digits"));

        return new SuccessfulResult<Cvv>(new Cvv(int.Parse(cvv)));
    }

    public static bool IsAmex(string cvv) => cvv.Length == AmericanExpressCvv;
    
    private static bool IsVisaMastercardOrAmex(string cvv) =>
        cvv.Length is VisaOrMastercardCvv or AmericanExpressCvv;
}