using System.Diagnostics;
using System.Runtime.InteropServices.Marshalling;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.ErrorHandling;

public static class ErrorHandlingExtensions
{
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.Run( async context =>
        {
            var logger = context.RequestServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("Global Error Handling");

            var exceptionsDetails = context.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionsDetails?.Error;

            logger.LogError(exception, "Could not process a request on machine {MachineName}. TraceId: {TraceId}", Environment.MachineName,
                Activity.Current?.TraceId);

            var problem = new ProblemDetails
            {
                Title = "We make a mistake but we are working on it!",
                Status = StatusCodes.Status500InternalServerError,
                Instance = context.Request.HttpContext.Request.Path,
                Extensions = new Dictionary<string, object?>
                {
                    {"traceId", Activity.Current?.TraceId.ToString()}
                }
            };

            var environment = context.RequestServices.GetRequiredService<IHostEnvironment>();
            if (environment.IsDevelopment())
            {
                problem.Detail = exception?.ToString();
            }


            await Results.Problem(problem).ExecuteAsync(context);
        });
    }
}