using System.Net;
using System.Text.Json;

namespace ZurichApp.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Ocurrió un error inesperado.";

            if (ex is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Forbidden;
                message = ex.Message;
            }
            else if (ex is KeyNotFoundException)
            {
                statusCode = (int)HttpStatusCode.NotFound;
                message = ex.Message;
            }
            else if (ex is ArgumentException || ex is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                message = ex.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = new
            {
                exito = false,
                mensaje = message,
                detalle = ex.Message,
                status = statusCode,
                traceId = context.TraceIdentifier
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
