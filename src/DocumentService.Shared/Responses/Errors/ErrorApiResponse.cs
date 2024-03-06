namespace DocumentService.Shared.Responses.Errors;

public class ErrorApiResponse
{
    public ErrorApiResponse(
        string code,
        string requestId,
        string message,
        string? details = null,
        IDictionary<string, object>? data = null)
    {
        Code = code;
        Message = message;
        RequestId = requestId;
        Data = data;
        Details = details;
    }

    /// <summary> Идентификатор ошибки </summary>
    public string Code { get; internal set; }

    /// <summary> Сообщение </summary>
    public string Message { get; internal set; }

    /// <summary> Подробности </summary>
    public string? Details { get; internal set; }

    /// <summary> Дополнительные свойства </summary>
    public IDictionary<string, object>? Data { get; internal set; }

    /// <summary> Идентификатор запроса </summary>
    public string RequestId { get; internal set; }
}
