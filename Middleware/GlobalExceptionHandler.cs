using System.Net;
using LifePet.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace LifePetApi.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Recurso não encontrado"),
            BusinessException => (HttpStatusCode.BadRequest, "Regra de negócio violada"),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "Não autorizado"),
            _ => (HttpStatusCode.InternalServerError, "Erro interno")
        };

        if (status == HttpStatusCode.InternalServerError)
            logger.LogError(exception, "Erro não tratado");
        else
            logger.LogWarning(exception, "Falha de negócio: {Message}", exception.Message);

        httpContext.Response.StatusCode = (int)status;
        await httpContext.Response.WriteAsJsonAsync(new
        {
            type = "about:blank",
            title,
            status = (int)status,
            detail = exception.Message
        }, cancellationToken);

        return true;
    }
}
