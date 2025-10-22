using System.Text.Json.Serialization;
using PgTempest.Sdk.Models;

namespace PgTempest.Sdk.Dtos;

internal sealed record StartTemplateInitializationResponseBody(
    [property: JsonPropertyName("initializationWasStarted")]
        StartTemplateInitializationResponseBodyInitializationWasStarted? InitializationWasStarted,
    [property: JsonPropertyName("initializationIsInProgress")]
        StartTemplateInitializationResponseBodyInitializationIsInProgress? InitializationIsInProgress,
    [property: JsonPropertyName("initializationIsInFinished")] EmptyDto? InitializationIsFinished,
    [property: JsonPropertyName("unexpectedError")]
        StartTemplateInitializationResponseBodyUnexpectedError? UnexpectedError
);

internal sealed record StartTemplateInitializationResponseBodyInitializationWasStarted(
    [property: JsonPropertyName("databaseConnectionOptions"), JsonRequired]
        DbConnectionOptionsDto DatabaseConnectionOptions,
    [property: JsonPropertyName("initializationDeadline"), JsonRequired]
        DateTime InitializationDeadline
);

internal sealed record StartTemplateInitializationResponseBodyInitializationIsInProgress(
    [property: JsonPropertyName("initializationDeadline"), JsonRequired]
        DateTime InitializationDeadline
);

internal sealed record StartTemplateInitializationResponseBodyUnexpectedError(
    [property: JsonPropertyName("message"), JsonRequired] string Message
);
