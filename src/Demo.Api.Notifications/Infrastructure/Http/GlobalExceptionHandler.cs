﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Api.Notifications.Domain;
using BuildingBlocks.Contracts.Exceptions;
using BuildingBlocks.Observability;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Api.Notifications.Infrastructure.Http;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Instance = httpContext.GetRequestEndpoint(),
            Extensions =
            {
                ["traceId"] = httpContext.GetTraceId()
            }
        };


        if(exception is UserNotFoundException || exception is NotificationNotFoundException)
        {
            problemDetails.Status = StatusCodes.Status404NotFound;
            problemDetails.Title = exception.Message;
            problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5";

            Activity.Current.RegisterValidation(exception);
        }
        else
        {
            problemDetails.Status = StatusCodes.Status500InternalServerError;
            problemDetails.Title = "An error occurred while processing your request";
            problemDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";

            _logger.LogError(
                exception,
                "An unhandled exception has occurred while executing the request.");

            Activity.Current.RegisterException(exception);
        }

        httpContext.Response.StatusCode = problemDetails.Status.Value;


        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
