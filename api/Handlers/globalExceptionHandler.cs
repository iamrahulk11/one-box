using domain.DTOs;
using helpers;
using System.Net;

namespace api.Handlers
{
    public class globalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<globalExceptionHandler> _logger;
        baseResponseDto<object> _baseResponse = new();
        resultResponseDto _resultResponse = new();

        public globalExceptionHandler(RequestDelegate next, ILogger<globalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
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


        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            _logger.LogError($"{ex.Message}\n{ex.StackTrace}\n\n");
            _resultResponse.flag = 0;
            _resultResponse.flag_message = responseMessages.SOMETHING_WENT_WRONG;
            _baseResponse.Result = _resultResponse;
            _baseResponse.Data = null;

            await context.Response.WriteAsJsonAsync(_baseResponse);
        }
    }

    public static class globalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalException(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<globalExceptionHandler>();
        }
    }
}