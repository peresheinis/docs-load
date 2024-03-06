using DocumentService.Shared.Responses.Errors;
using System.Text.Json;

namespace DocumentService.Web.Exceptions;

public class ApiResponseException : Exception
{
    public ApiResponseException(string host, string path)
        : this("Неожиданный ответ от Api", host, path)
    {
    }

    public ApiResponseException(string message, string host, string path)
        : base(message)
    {
        Host = host;
        Path = path;
    }

    public ApiResponseException(string message, HttpResponseMessage httpResponseMessage)
        : this(message,
            httpResponseMessage.RequestMessage.RequestUri?.Host ?? "NotSet",
            httpResponseMessage.RequestMessage.RequestUri?.PathAndQuery ?? "NotSet")
    {
    }

    public ApiResponseException(ErrorApiResponse errorApiResponse, HttpResponseMessage httpResponseMessage) : this(
        errorApiResponse.Message, httpResponseMessage)
    {
        ErrorApiResponse = errorApiResponse;
        RequestId = errorApiResponse.RequestId;
    }

    public ErrorApiResponse ErrorApiResponse { get; private set; }
    public string Host { get; }
    public string Path { get; }
    public string RequestId { get; }

    public static async Task EnsureSuccessAsync(HttpResponseMessage httpResponseMessage)
    {
        if (httpResponseMessage.IsSuccessStatusCode is false)
        {
            var responseStr = await httpResponseMessage.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseStr))
                throw new ApiResponseException("Неожиданный ответ от API", httpResponseMessage);

            var errorApiResponse = JsonSerializer.Deserialize<ErrorApiResponse>(responseStr);

            if (errorApiResponse is null)
                throw new ApiResponseException("Неожиданный ответ от API", httpResponseMessage);

            throw new ApiResponseException(errorApiResponse, httpResponseMessage);
        }
    }

    public static void EnsureSuccess(HttpResponseMessage httpResponseMessage, string json)
    {
        if (httpResponseMessage.IsSuccessStatusCode is false)
        {
            if (string.IsNullOrEmpty(json))
                throw new ApiResponseException("Неожиданный ответ от API", httpResponseMessage);

            var errorApiResponse = JsonSerializer.Deserialize<ErrorApiResponse>(json);
            if (errorApiResponse is null)
                throw new ApiResponseException("Неожиданный ответ от API", httpResponseMessage);
            else
                throw new ApiResponseException(errorApiResponse, httpResponseMessage);
        }
    }
}
