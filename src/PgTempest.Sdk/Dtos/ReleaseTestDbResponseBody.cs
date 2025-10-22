using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Dtos;

internal sealed record ReleaseTestDbResponseBody(
    [property: JsonPropertyName("testDbWasReleased")] EmptyDto? TestDbWasReleased,
    [property: JsonPropertyName("templateWasNotFound")] EmptyDto? TemplateWasNotFound,
    [property: JsonPropertyName("testDbWasNotFound")] EmptyDto? TestDbWasNotFound
);
