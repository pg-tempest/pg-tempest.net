using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Client.Dtos;

internal sealed record FinishTemplateInitializationResponseBody(
    [property: JsonPropertyName("initializationIsFinished")] EmptyDto? InitializationIsFinished,
    [property: JsonPropertyName("templateWasNotFound")] EmptyDto? TemplateWasNotFound,
    [property: JsonPropertyName("initializationIsFailed")] EmptyDto? InitializationIsFailed
);
