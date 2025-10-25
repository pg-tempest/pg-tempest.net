using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Client.Dtos;

internal sealed record FailTemplateInitializationResponseBody(
    [property: JsonPropertyName("initializationIsFailed")] EmptyDto? InitializationIsFailed,
    [property: JsonPropertyName("templateWasNotFound")] EmptyDto? TemplateWasNotFound,
    [property: JsonPropertyName("initializationIsFinished")] EmptyDto? InitializationIsFinished
);
