using PgTempest.Sdk.Models;

namespace PgTempest.Sdk.Client.Models;

public abstract record StartTemplateInitializationResult
{
    public sealed record InitializationWasStarted(
        DbConnectionOptions DbConnectionOptions,
        DateTime InitializationDeadline
    ) : StartTemplateInitializationResult;

    public sealed record InitializationIsInProgress(DateTime InitializationDeadline)
        : StartTemplateInitializationResult;

    public sealed record InitializationIsFinished : StartTemplateInitializationResult;
}
