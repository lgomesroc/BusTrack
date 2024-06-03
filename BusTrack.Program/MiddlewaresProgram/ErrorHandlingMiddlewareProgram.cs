namespace BusTrack.BusTrack.Program.MiddlewaresProgram
{
    public class ErrorHandlingMiddlewareProgram
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddlewareProgram> _logger;

        public ErrorHandlingMiddlewareProgram(RequestDelegate next, ILogger<ErrorHandlingMiddlewareProgram> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado.");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync("Ocorreu um erro inesperado. Por favor, tente novamente mais tarde.");
            }
        }
    }
}
