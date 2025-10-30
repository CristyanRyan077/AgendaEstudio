using System.Diagnostics;

namespace AgendaApi.Extensions.MiddleWares
{
    public class RequestTimerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestTimerMiddleWare> _logger;

        public RequestTimerMiddleWare(RequestDelegate next, ILogger<RequestTimerMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();

            await _next(context);

            sw.Stop();
            var route = context.Request.Path;
            var method = context.Request.Method;
            var elapsed = sw.ElapsedMilliseconds;

            _logger.LogInformation("[TIMER] {Method} {Route} -> {Elapsed} ms", method, route, elapsed);
        }
    }
}
