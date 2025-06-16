namespace PIGGISWS.Services.Utils;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Continúa con la ejecución del pipeline
            await _next(context);
        }
        catch (Exception ex)
        {
            // Maneja la excepción
            _logger.LogError(ex, " **** ERROR ****  Ocurrió un error no controlado.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Devuelve un mensaje de error genérico al cliente
        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "**** ERROR ****  Se produjo un error interno en el servidor. Por favor, contacte al administrador.",
            Details = exception.Message 
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}