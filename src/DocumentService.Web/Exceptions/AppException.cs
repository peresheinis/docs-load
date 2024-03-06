namespace DocumentService.Web.Exceptions;

/// <summary>Базовый класс ошибок приложения, не зависящих от пользователя</summary>
public abstract class AppException : Exception
{
    protected AppException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}
