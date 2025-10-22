using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Client.Dtos;

internal sealed record StartTemplateInitializationRequestBody(
    [property: JsonPropertyName("templateHash")] string TemplateHash,
    [property: JsonPropertyName("initializationDurationInSeconds")]
        ulong InitializationDurationInSeconds
);
