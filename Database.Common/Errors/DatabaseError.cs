using Common.Errors;

namespace Database.Common.Errors;

public class DatabaseError : Error
{
    public DatabaseError(string errorMessage) : base(ErrorCode.DatabaseError, errorMessage)
    { }
}