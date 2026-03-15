using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;

namespace ThePredictions.Web.Client.Components.Shared;

public class LoggingErrorBoundary : ErrorBoundary
{
    [Inject]
    private ILogger<LoggingErrorBoundary> Logger { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "Unhandled error caught by ErrorBoundary: {ErrorMessage}", exception.Message);
        return Task.CompletedTask;
    }
}
