using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Dtos;

internal sealed record GetTestDbRequestBody(
    [property: JsonPropertyName("templateHash")] string TemplateHash,
    [property: JsonPropertyName("usageDurationInSeconds")] ulong UsageDurationInSeconds
);
