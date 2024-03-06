using Microsoft.Extensions.Primitives;
using Serilog;
using System.Security.Claims;

namespace DocumentService.Web.Extensions
{
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Enriches the HTTP request log with additional data via the Diagnostic Context
        /// </summary>
        /// <param name="diagnosticContext">The Serilog diagnostic context</param>
        /// <param name="httpContext">The current HTTP Context</param>
        public static void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            diagnosticContext.Set("ClientIP", httpContext.GetUserIp());
            diagnosticContext.Set("UserAgent", httpContext.GetUserAgent());
            diagnosticContext.Set("UserId", httpContext.GetUserId());

            if (httpContext.Request.Method == "GET") diagnosticContext.Set("QueryString", httpContext.Request.QueryString);
        }

        public static string? GetUserId(this HttpContext context)
        {
            return context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
        }

        public static Guid GetUserGuid(this HttpContext context)
        {
            Guid.TryParse(context.GetUserId(), out var userId);
            return userId;
        }

        public static string? GetUserAgent(this HttpContext context)
        {
            return context.Request.Headers["User-Agent"].FirstOrDefault();
        }

        public static string GetUserIp(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
