namespace DocumentService.Web.Exceptions;

public static class ExceptionCodes
{
    public const string EntityNotFound = "EntityNotFound";
    public const string DataInvalid = "DataInvalid";
    public const string Conflict = "Conflict";
    public const string UnderTransactionConflict = "UnderTransactionConflict";
    public const string EntityAlreadyExistConflict = "EntityAlreadyExistConflict";
    public const string PermissionDenied = "PermissionDenied";
    public const string TooManyRequests = "TooManyRequests";
    public const string AuthError = "AuthError";
}
