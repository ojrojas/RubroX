using System.Net;
using System.Text.Json;
using RubroX.Domain.Exceptions;

namespace RubroX.API.Middleware;

/// <summary>Middleware global de manejo de excepciones. Convierte excepciones a respuestas HTTP consistentes.</summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción no controlada en {Path}", context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = exception switch
        {
            DomainException => (HttpStatusCode.UnprocessableEntity, "Error de dominio"),
            SaldoInsuficienteException => (HttpStatusCode.Conflict, "Saldo insuficiente"),
            RubroNoEncontradoException => (HttpStatusCode.NotFound, "Recurso no encontrado"),
            FlujoInvalidoException => (HttpStatusCode.BadRequest, "Flujo inválido"),
            _ => (HttpStatusCode.InternalServerError, "Error interno del servidor")
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)statusCode;

        var problem = new
        {
            type = $"https://rubrox.gov.co/errors/{statusCode}",
            title,
            status = (int)statusCode,
            detail = exception.Message,
            instance = context.Request.Path.Value
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
    }
}
