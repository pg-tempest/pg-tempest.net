using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Client.Dtos;

internal sealed record FailTemplateInitializationRequestBody(
    [property: JsonPropertyName("templateHash")] string TemplateHash
);
