using System.Text.RegularExpressions;
using Common;
using Domain.Common.Errors;

namespace Domain.ValueObjects;

public class Month
{
    public int Get { get; }
    
    private Month(int month)
    {
        Get = month;
    }

    public static Result<Month> Create(string month)
    {
        if (string.IsNullOrWhiteSpace(month))
            return new ErrorResult<Month>(new DateError("Month is empty"));
        
        var isNumber = int.TryParse(month, out var maybeNumber);

        if (isNumber)
        {
            if(maybeNumber is >= 0 and <= 12)
                return new SuccessfulResult<Month>(new Month(maybeNumber));
            
            return new ErrorResult<Month>(new DateError("Month is not in range"));
        }

        if (!Regex.IsMatch(month, @"^(?i)(January|February|March|April|May|June|July|August|September|October|November|December)$"))
            return new ErrorResult<Month>(new DateError("Month is not in range"));
        
        return new SuccessfulResult<Month>(new Month(FromString(month)));
    }

    private static int FromString(string month)
    {
        switch (month)
        {
            case "january":
            case "January":
                return 1;
            case "february":
            case "February":
                return 2;
            case "march":
            case "March":
                return 3;
            case "april":
            case "April":
                return 4;
            case "may":
            case "May":
                return 5;
            case "june":
            case "June":
                return 6;
            case "july":
            case "July":
                return 7;
            case "august":
            case "August":
                return 8;
            case "september":
            case "September":
                return 9;
            case "october":
            case "October":
                return 10;
            case "november":
            case "November":
                return 11;
            case "december":
            case "December":
                return 12;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override string ToString()
    {
        return Get switch
        {
            1 => "January",
            2 => "February",
            3 => "March",
            4 => "April",
            5 => "May",
            6 => "June",
            7 => "July",
            8 => "August",
            9 => "September",
            10 => "October",
            11 => "November",
            12 => "December",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}