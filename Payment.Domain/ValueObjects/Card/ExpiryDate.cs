using Common;
using Domain.Common.Errors;

namespace Domain.ValueObjects;

public class ExpiryDate
{
    public Month Month { get; }
    public Year Year { get; }
    
    private ExpiryDate(Month month, Year year)
    {
        Month = month;
        Year = year;
    }

    public static Result<ExpiryDate> Create(Month month, Year year)
    {
        var today = DateTime.Today;
        var currentMonth = today.Month;
        var currentYear = today.Year;

        // expiry date is in the future
        if (year.Get > currentYear)
            return new SuccessfulResult<ExpiryDate>(new ExpiryDate(month, year));

        // expiry date is during the current year
        if (year.Get == currentYear)
        {
            // expiry date is during the current month and year
            if (month.Get >= currentMonth)
            {
                return new SuccessfulResult<ExpiryDate>(new ExpiryDate(month, year));
            }

            return new ErrorResult<ExpiryDate>(new ExpiryDateError("The expiry date is in the past"));
        }

        // expiry date is in the past
        return new ErrorResult<ExpiryDate>(new ExpiryDateError("The expiry date is in the past"));
    }
}