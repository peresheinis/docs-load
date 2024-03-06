using DocumentService.Web.Exceptions;

namespace DocumentService.Web.Extensions;

public static class ConflictExceptionExtensions
{
    public static void ThrowNotFoundIfNull<T>(this T target, string details)
    {
        if (target == null)
            throw ConflictException.NotFound(details);
    }
}
