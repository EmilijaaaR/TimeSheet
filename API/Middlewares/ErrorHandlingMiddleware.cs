using Application.Exceptions;
namespace API.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                InvalidCredentialsException => (StatusCodes.Status401Unauthorized, ex.Message),
                ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = statusCode;

            var response = new
            {
                error = message
            };

            return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(message));
        }
    }

}
