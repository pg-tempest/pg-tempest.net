using System;
using Dunet;

namespace PgTempest.Sdk.Models;

[Union]
public partial record StartTemplateInitializationResult
{
    public partial record InitializationWasStarted(
        DbConnectionOptions DbConnectionOptions,
        DateTime InitializationDeadline
    );

    public partial record InitializationIsInProgress(DateTime InitializationDeadline);

    public partial record InitializationIsFinished();
}
