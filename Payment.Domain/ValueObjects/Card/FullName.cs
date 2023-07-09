using System.Text.RegularExpressions;
using Common;
using Domain.Common.Errors;

namespace Domain.ValueObjects;

public class FullName
{
    public string Get { get; }

    private FullName(string fullName)
    {
        Get = fullName;
    }

    public static Result<FullName> Create(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return new ErrorResult<FullName>(new FullNameError("Full name is empty"));

        if (fullName.Split(' ').Length == 1)
            return new ErrorResult<FullName>(new FullNameError("Fullname must contain a first and a last name"));

        if(!Regex.IsMatch(fullName, @"^[a-zA-Z\s]+$"))
            return new ErrorResult<FullName>(new FullNameError("Full name can contain letters only"));
        
        return new SuccessfulResult<FullName>(new FullName(fullName));
    }
}