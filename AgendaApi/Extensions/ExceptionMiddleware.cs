using System.Net;

namespace AgendaApi.Extensions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro capturado no middleware");

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = ex switch
                {
                    NotFoundException => (int)HttpStatusCode.NotFound,
                    ValidationException => (int)HttpStatusCode.BadRequest,
                    BusinessException => (int)HttpStatusCode.UnprocessableEntity,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var response = new
                {
                    status = context.Response.StatusCode,
                    message = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError && !_env.IsDevelopment()
                     ? "Ocorreu um erro interno no servidor."
                     : ex.Message,
                    errorType = ex.GetType().Name
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
