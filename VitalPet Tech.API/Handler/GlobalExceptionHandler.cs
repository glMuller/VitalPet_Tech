using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VitalPetTech.Application.Exceptions;

namespace VitalPet_Tech.API.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado."),
            BusinessRuleException => (StatusCodes.Status422UnprocessableEntity, "Regra de negócio violada."),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            _logger.LogError(exception, "Erro não tratado em {Path}.", httpContext.Request.Path);
        else
            _logger.LogWarning("{Titulo} {Mensagem}", title, exception.Message);

        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            // Nunca expõe detalhes internos em erros 500
            Detail = status == StatusCodes.Status500InternalServerError ? null : exception.Message,
            Instance = httpContext.Request.Path
        };
        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}