using System.Text.RegularExpressions;
using Common;
using Domain.Common.Errors;

namespace Domain.ValueObjects;

public class Year
{
    public int Get { get; }
    
    private Year(int year)
    {
        Get = year;
    }

    public static Result<Year> Create(string year)
    {
        if (string.IsNullOrWhiteSpace(year))
            return new ErrorResult<Year>(new DateError("Year is empty"));

        if(year.Length != 4)
            return new ErrorResult<Year>(new DateError("Year must contain 4 digits"));

        if (!Regex.IsMatch(year, @"^[0-9]+$"))
            return new ErrorResult<Year>(new DateError("Year can contain digits only"));

        return new SuccessfulResult<Year>(new Year(int.Parse(year)));
    }
}